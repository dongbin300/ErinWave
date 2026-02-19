using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ErinWave.GooglePlayPaymentsManager
{
    public class RealPaymentParser
    {
        private string HtmlDecode(string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
                return encoded;

            return encoded.Replace("&quot;", "\"")
                          .Replace("&amp;", "&")
                          .Replace("&lt;", "<")
                          .Replace("&gt;", ">")
                          .Replace("&apos;", "'");
        }

        public List<PaymentItem> ParsePaymentsFile(string filePath)
        {
            var payments = new List<PaymentItem>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Payment file not found: {filePath}");
            }

            Console.WriteLine("Starting to parse file: " + filePath);

            string htmlContent = File.ReadAllText(filePath);
            Console.WriteLine("File size: " + htmlContent.Length + " characters");

            // 파일을 <tr> 태그로 분리하여 각 행을 처리
            var rows = ExtractPaymentRows(htmlContent);
            Console.WriteLine("Found " + rows.Count + " payment rows");

            int successCount = 0;
            foreach (var row in rows)
            {
                var payment = ParsePaymentRow(row);
                if (payment != null)
                {
                    payments.Add(payment);
                    successCount++;
                    Console.WriteLine("✓ (" + successCount + ") " + payment.Date + " - " + payment.ProductName + " - " + payment.FormattedAmount);
                }
            }

            Console.WriteLine("Successfully parsed " + successCount + " payments out of " + rows.Count + " rows");
            return payments;
        }

        private List<string> ExtractPaymentRows(string htmlContent)
        {
            var rows = new List<string>();

            // <tr 태그로 각 행을 분리
            var trPattern = @"<tr[^>]*class=""b3id-widget-table-data-row[^""]*""[^>]*>.*?</tr>";
            var matches = Regex.Matches(htmlContent, trPattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                rows.Add(match.Value);
            }

            return rows;
        }

        private PaymentItem ParsePaymentRow(string rowHtml)
        {
            try
            {
                // 상품명 추출 - 두 번째 body2에서 찾기
                var productPattern = @"data-info-message=""\[\&quot;([^&]+?)\s*·\s*([^&]+?)\&quot;[^""]*body2[^""]*""[^""]*>\[^<]*<span[^>]*>([^<]+)</span>";
                var productMatch = Regex.Match(rowHtml, productPattern);

                if (!productMatch.Success)
                {
                    // 대체 패턴으로 시도
                    productPattern = @"body2.*?data-info-message=""\[\&quot;([^&]+?)\s*·\s*([^&]+?)\&quot;";
                    productMatch = Regex.Match(rowHtml, productPattern);
                }

                if (!productMatch.Success)
                {
                    Console.WriteLine("Could not find product pattern");
                    return null;
                }

                var date = HtmlDecode(productMatch.Groups[1].Value.Trim());
                var productName = HtmlDecode(productMatch.Groups[2].Value.Trim());

                // 금액 추출 - amount-debit 클래스에서 찾기
                var amountPattern = @"info-message-amount-debit[^>]*>.*?data-info-message=""\[\&quot;(-?₩[\d,]+)\&quot;";
                var amountMatch = Regex.Match(rowHtml, amountPattern);

                if (!amountMatch.Success)
                {
                    // 대체 패턴으로 시도
                    amountPattern = @"amount-debit.*?data-info-message=""\[\&quot;(-?₩[\d,]+)\&quot;";
                    amountMatch = Regex.Match(rowHtml, amountPattern);
                }

                if (!amountMatch.Success)
                {
                    Console.WriteLine("Could not find amount for: " + date + " - " + productName);
                    return null;
                }

                var amountText = HtmlDecode(amountMatch.Groups[1].Value.Trim());

                // 이미지 URL 추출
                var imagePattern = @"<img[^>]*src=""([^""]+)""[^>]*>";
                var imageMatch = Regex.Match(rowHtml, imagePattern);
                var imageUrl = imageMatch.Success ? imageMatch.Groups[1].Value : "";

                var payment = new PaymentItem
                {
                    Store = "Google Play 앱",
                    Date = date,
                    ProductName = productName,
                    FormattedAmount = amountText,
                    Currency = "₩",
                    ImageUrl = imageUrl
                };

                // 금액 파싱
                var cleanAmount = amountText.Replace("₩", "").Replace(",", "").Replace("-", "");
                if (decimal.TryParse(cleanAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
                {
                    payment.Amount = amountText.StartsWith("-") ? -amount : amount;
                }

                return payment;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing payment row: " + ex.Message);
                return null;
            }
        }
    }
}