using System;
using System.Runtime.InteropServices;

namespace ErinWave.NeuroExposePcSound;

public static class VolumeControlHelper
{
	// ====== 1. 상수 정의 (Core Audio API GUIDs) ======

	// IMMDeviceEnumerator의 클래스 ID (CLSID)
	private static readonly Guid CLSID_MMDeviceEnumerator =
		new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E");

	// IAudioEndpointVolume 인터페이스 ID (IID)
	private static readonly Guid IID_IAudioEndpointVolume =
		new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");

	// 상수 정의: CLSCTX_ALL
	private const uint CLSCTX_ALL = 0x17;

	// ====== 2. COM 인터페이스 정의 ======

	// 데이터 흐름 지정 (eRender = 출력 장치)
	public enum EDataFlow { eRender, eCapture, eAll };
	// 장치 상태 지정 (DEVICE_STATE_ACTIVE = 장치 활성화)
	public enum DEVICE_STATE { DEVICE_STATE_ACTIVE = 0x00000001 };

	// IMMDeviceEnumerator 인터페이스 (EnumAudioEndpoints 제거)
	[ComImport]
	[Guid("A95664D2-9614-4F35-A746-DE8DB636FD7C")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMMDeviceEnumerator
	{
		// EnumAudioEndpoints는 사용하지 않으므로 생략 (HResult 반환을 위해 int로 선언)
		[PreserveSig]
		int EnumAudioEndpoints(EDataFlow dataFlow, DEVICE_STATE dwStateMask, out IntPtr ppDevices); // IntPtr로 대체

		// GetDefaultAudioEndpoint만 남김
		int GetDefaultAudioEndpoint(EDataFlow dataFlow, DEVICE_STATE dwStateMask, out IMMDevice ppEndpoint);
	}

	// IMMDevice 인터페이스
	[ComImport]
	[Guid("D666063F-158E-4E75-99C8-2A000A15E86E")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMMDevice
	{
		int Activate(ref Guid iid, uint dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
		// ... (나머지 메서드는 필요 없으므로 생략)
	}

	// IAudioEndpointVolume 인터페이스
	[ComImport]
	[Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IAudioEndpointVolume
	{
		// SetMute까지의 9개 메서드는 사용하지 않더라도 순서 유지를 위해 정의합니다.
		[PreserveSig] int Not_Used_1();
		[PreserveSig] int Not_Used_2();
		[PreserveSig] int Not_Used_3();
		[PreserveSig] int Not_Used_4();
		[PreserveSig] int Not_Used_5();
		[PreserveSig] int Not_Used_6();
		[PreserveSig] int Not_Used_7();
		[PreserveSig] int Not_Used_8();
		[PreserveSig] int Not_Used_9();

		// Mute 설정 메서드
		int SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, ref Guid pguidEventContext);

		// Mute 상태 가져오기 메서드 (옵션)
		int GetMute([MarshalAs(UnmanagedType.Bool)] out bool pbMute);
	}

	// ====== 3. 메인 기능 구현 ======

	public static void SetSystemMasterMute(bool mute)
	{
		object enumeratorObject = null;
		IMMDeviceEnumerator deviceEnumerator = null;
		IMMDevice defaultDevice = null;
		object volumeObject = null;
		IAudioEndpointVolume volumeControl = null;

		try
		{
			// 1. IMMDeviceEnumerator 객체 생성 (COM API 호출)
			Type enumeratorType = Type.GetTypeFromCLSID(CLSID_MMDeviceEnumerator);
			enumeratorObject = Activator.CreateInstance(enumeratorType);
			deviceEnumerator = (IMMDeviceEnumerator)enumeratorObject;

			// 2. 기본 오디오 출력 장치 가져오기 (eRender = 출력, DEVICE_STATE_ACTIVE = 활성 장치)
			deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE, out defaultDevice);

			// 3. IMMDevice로부터 IAudioEndpointVolume 인터페이스 활성화
			Guid iidAudioEndpointVolume = IID_IAudioEndpointVolume;
			defaultDevice.Activate(ref iidAudioEndpointVolume, CLSCTX_ALL, IntPtr.Zero, out volumeObject);
			volumeControl = (IAudioEndpointVolume)volumeObject;

			// 4. Mute 상태 설정 (True 또는 False)
			Guid guid = Guid.Empty;
			volumeControl.SetMute(mute, ref guid);
		}
		finally
		{
			// 5. COM 객체 정리 (매우 중요!)
			if (volumeControl != null && Marshal.IsComObject(volumeControl)) Marshal.ReleaseComObject(volumeControl);
			if (volumeObject != null && Marshal.IsComObject(volumeObject)) Marshal.ReleaseComObject(volumeObject);
			if (defaultDevice != null && Marshal.IsComObject(defaultDevice)) Marshal.ReleaseComObject(defaultDevice);
			if (deviceEnumerator != null && Marshal.IsComObject(deviceEnumerator)) Marshal.ReleaseComObject(deviceEnumerator);
			if (enumeratorObject != null && Marshal.IsComObject(enumeratorObject)) Marshal.ReleaseComObject(enumeratorObject);
		}
	}
}