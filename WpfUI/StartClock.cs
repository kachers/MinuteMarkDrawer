using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using System.Drawing.Text;

namespace WpfUI
{
    public class StartClock
    {
        private CancellationTokenSource cancellationTokenSource;
        public void GeneratePng(string sourceFolder)
        {
            // Create a CancellationTokenSource for StopGeneratePng method
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            string backgroundImagePath = sourceFolder + "/background.jpg";
            DateTime startTime = DateTime.Now;
            int minutesPassed = 0;

            do
            {
                Random random = new Random();
                Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                SolidBrush brush = new SolidBrush(randomColor);

                if (DateTime.Now - startTime >= TimeSpan.FromMinutes(1) || minutesPassed == 0)
                {
                    UpdateJsonFile(sourceFolder, randomColor);
                    startTime = DateTime.Now;
                    Size newSize = new Size(240, 280);
                    Bitmap resizedImage = new Bitmap(newSize.Width, newSize.Height);
                    Font font = LoadAndUseFont(sourceFolder, "neon.ttf", 55);

                    using (Bitmap originalImage = new Bitmap(backgroundImagePath))
                    {
                        // Draw the original image onto the resized image
                        using (Graphics graphics = Graphics.FromImage(resizedImage))
                        {
                            graphics.DrawImage(originalImage, new Rectangle(Point.Empty, newSize));

                            TimeSpan currentSystemTime = DateTime.Now.TimeOfDay;
                            string currentTime = currentSystemTime.ToString(@"hh\:mm");
                            // Calculate the position to center the text
                            SizeF textSize = graphics.MeasureString(currentTime, font);
                            float x = (newSize.Width - textSize.Width) / 2;
                            float y = (newSize.Height - textSize.Height) / 2;

                            PointF textPosition = new PointF(x, y);
                            graphics.DrawString(currentTime, font, brush, textPosition);
                        }

                        string outputPath = sourceFolder + "/image.png";
                        resizedImage.Save(outputPath, ImageFormat.Png);
                    }

                    minutesPassed++;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                Thread.Sleep(500);
            } while (true);
        }

        public void StopGeneratePng()
        {
            cancellationTokenSource?.Cancel();
        }

        public void UpdateJsonFile(string sourceFolder, Color color)
        {
            string jsonFilePath = sourceFolder + "/led_config.json";
            string jsonContent = File.ReadAllText(jsonFilePath);
            dynamic jsonObj = JsonConvert.DeserializeObject(jsonContent);
            string rgbValue = $"{color.R:X2}{color.G:X2}{color.B:X2}";
            jsonObj.color = rgbValue;
            string updatedJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(jsonFilePath, updatedJson);
        }

        public Font LoadAndUseFont(string sourceFolder, string fontFileName, float fontSize)
        {
            string fontFilePath = Path.Combine(sourceFolder, fontFileName);
            PrivateFontCollection fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(fontFilePath);

            FontFamily fontFamily = fontCollection.Families[0];
            Font userFont = new Font(fontFamily, fontSize);

            return userFont;
        }
    }
}
