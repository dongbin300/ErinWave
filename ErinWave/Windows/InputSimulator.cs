//-----------------------------------------------------------------------
//
// MIT License
//
// Copyright (c) 2025 Erin Wave
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//-----------------------------------------------------------------------

using ErinWave.Windows;

using InputSimulatorStandard;
using InputSimulatorStandard.Native;

namespace ErinWave.Windows
{
    public class InputSimulator
    {
        public static int MouseActivityInterval = 0;
        public static int KeyboardActivityInterval = 0;
        private static readonly KeyboardSimulator keyboardSimulator = new();
        private static readonly MouseSimulator mouseSimulator = new();

        public static void MouseMove(int x, int y) => WinApi.SetCursorPos(x, y);
        public static void MouseClick() => mouseSimulator.LeftButtonClick();
        public static void MouseClick(int x, int y)
        {
            MouseMove(x, y);
            Thread.Sleep(MouseActivityInterval);
            mouseSimulator.LeftButtonClick();
        }
        public static void MouseDoubleClick() => mouseSimulator.LeftButtonDoubleClick();
        public static void MouseDoubleClick(int x, int y)
        {
            MouseMove(x, y);
            Thread.Sleep(MouseActivityInterval);
            mouseSimulator.LeftButtonDoubleClick();
        }
        public static void MouseRightClick() => mouseSimulator.RightButtonClick();
        public static void MouseRightClick(int x, int y)
        {
            MouseMove(x, y);
            Thread.Sleep(MouseActivityInterval);
            mouseSimulator.RightButtonClick();
        }
        /// <summary>
        /// 마우스 스크롤
        /// +: 위로 올리기
        /// -: 아래로 내리기
        /// </summary>
        /// <param name="scrollNum"></param>
		public static void MouseScroll(int scrollNum) => mouseSimulator.VerticalScroll(scrollNum);
		public static void KeyPress(VirtualKeyCode keyCode) => keyboardSimulator.KeyPress(keyCode);
        public static void KeyPress(Modifiers modifiers, VirtualKeyCode keyCode) 
        {
            if (modifiers.HasFlag(Modifiers.Ctrl))
            {
                keyboardSimulator.KeyDown(VirtualKeyCode.CONTROL);
            }
            if (modifiers.HasFlag(Modifiers.Alt))
            {
                keyboardSimulator.KeyDown(VirtualKeyCode.MENU);
            }
            if (modifiers.HasFlag(Modifiers.Shift))
            {
                keyboardSimulator.KeyDown(VirtualKeyCode.SHIFT);
            }
            if (modifiers.HasFlag(Modifiers.Window))
            {
                keyboardSimulator.KeyDown(VirtualKeyCode.LWIN);
            }
            Thread.Sleep(KeyboardActivityInterval);
            keyboardSimulator.KeyPress(keyCode);
            Thread.Sleep(KeyboardActivityInterval);
            if (modifiers.HasFlag(Modifiers.Ctrl))
            {
                keyboardSimulator.KeyUp(VirtualKeyCode.CONTROL);
            }
            if (modifiers.HasFlag(Modifiers.Alt))
            {
                keyboardSimulator.KeyUp(VirtualKeyCode.MENU);
            }
            if (modifiers.HasFlag(Modifiers.Shift))
            {
                keyboardSimulator.KeyUp(VirtualKeyCode.SHIFT);
            }
            if (modifiers.HasFlag(Modifiers.Window))
            {
                keyboardSimulator.KeyUp(VirtualKeyCode.LWIN);
            }
        }
        public static void Sleep(int milliseconds) => Thread.Sleep(TimeSpan.FromMilliseconds(milliseconds));
    }

    [Flags]
    public enum Modifiers
    {
        None = 0x0,
        Ctrl = 0x1,
        Alt = 0x2,
        Shift = 0x4,
        Window = 0x8
    }
}
