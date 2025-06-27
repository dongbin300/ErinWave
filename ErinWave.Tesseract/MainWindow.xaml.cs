using OpenCvSharp;

using Tesseract;

namespace ErinWave.Tesseract
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : System.Windows.Window
	{
		public MainWindow()
		{
			InitializeComponent();

			string tesseractDataPath = @"C:\Program Files\Tesseract-OCR\tessdata";
			string imagePath = @"C:\book\dev (10).jpg";

			try
			{
				Mat img = Cv2.ImRead(imagePath, ImreadModes.Grayscale); // 흑백 변환
				Cv2.GaussianBlur(img, img, new OpenCvSharp.Size(3, 3), 0); // 가우시안 블러
				Cv2.AdaptiveThreshold(img, img, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 15, 10); // 이진화

				// 전처리된 이미지 저장 (디버깅용)
				Cv2.ImWrite("processed_image.png", img);

				// OCR 처리
				using (var ocrEngine = new TesseractEngine(tesseractDataPath, "kor+eng", EngineMode.Default))
				{
					using (var page = ocrEngine.Process(Pix.LoadFromFile("processed_image.png")))
					{
						string extractedText = page.GetText();
						System.IO.File.WriteAllText("output_text.txt", extractedText);
						Console.WriteLine("추출된 텍스트:\n" + extractedText);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"오류 발생: {ex.Message}");
			}
		}
	}
}