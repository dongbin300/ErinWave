using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ErinWave.GooglePlayPaymentsManager
{
    public class HtmlPaymentParser
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

            string htmlContent = File.ReadAllText(filePath);
            return ParseHtmlContent(htmlContent);
        }

        private List<PaymentItem> ParseHtmlContent(string htmlContent)
        {
            var payments = new List<PaymentItem>();

            // tbody 안의 모든 tr 태그들을 추출
            var tbodyPattern = @"<tbody[^>]*>(.*?)</tbody>";
            var tbodyMatch = Regex.Match(htmlContent, tbodyPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (!tbodyMatch.Success)
            {
                Console.WriteLine("No tbody found in HTML content");
                return payments;
            }

            var tbodyContent = tbodyMatch.Groups[1].Value;

            // 테이블 행들을 추출 (tr 태그) - 실제 클래스명에 맞게 수정
            var rowPattern = @"<tr[^>]*class=""b3id-widget-table-data-row[^""]*""[^>]*>(.*?)</tr>";
            var rows = Regex.Matches(tbodyContent, rowPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            Console.WriteLine($"Found {rows.Count} rows in HTML content");

            foreach (Match rowMatch in rows)
            {
                var rowHtml = rowMatch.Groups[1].Value;
                var payment = ExtractPaymentFromRow(rowHtml);

                if (payment != null)
                {
                    payments.Add(payment);
                }
            }

            return payments;
        }

        private PaymentItem ExtractPaymentFromRow(string rowHtml)
        {
            try
            {
                var payment = new PaymentItem();

                // 상점 이름 추출 - data-info-message에서 Google Play 앱 찾기
                var storePattern = @"data-info-message=""\[\&quot\;Google Play 앱\&quot\;";
                var storeMatch = Regex.Match(rowHtml, storePattern);
                payment.Store = storeMatch.Success ? "Google Play 앱" : "알 수 없음";

                // 날짜와 상품명 추출 - 정확한 패턴으로 수정
                var infoPattern = @"data-info-message=""\[\&quot\;([^&]*?)\s*·\s*([^&]*?)\&quot\;";
                var infoMatch = Regex.Match(rowHtml, infoPattern);
                if (infoMatch.Success)
                {
                    var rawDate = infoMatch.Groups[1].Value;
                    var rawProduct = infoMatch.Groups[2].Value;

                    payment.Date = HtmlDecode(rawDate.Trim());
                    payment.ProductName = HtmlDecode(rawProduct.Trim());

                    Console.WriteLine($"Parsed: {payment.Date} - {payment.ProductName}");
                }
                else
                {
                    Console.WriteLine("Failed to match date/product pattern");
                }

                // 금액 추출 - 정확한 패턴으로 수정
                var amountPattern = @"data-info-message=""\[\&quot\;(-?₩[\d,]+)\&quot\;";
                var amountMatch = Regex.Match(rowHtml, amountPattern);
                if (amountMatch.Success)
                {
                    var amountText = HtmlDecode(amountMatch.Groups[1].Value);
                    payment.FormattedAmount = amountText;

                    // 금액 파싱
                    var cleanAmount = amountText.Replace("₩", "").Replace(",", "").Replace("-", "");
                    if (decimal.TryParse(cleanAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
                    {
                        payment.Amount = amountText.StartsWith("-") ? -amount : amount;
                    }
                    payment.Currency = "₩";

                    Console.WriteLine($"Parsed amount: {payment.FormattedAmount} -> {payment.Amount}");
                }
                else
                {
                    Console.WriteLine("Failed to match amount pattern");
                }

                // 이미지 URL 추출
                var imagePattern = @"<img[^>]*src=""([^""]+)""[^>]*>";
                var imageMatch = Regex.Match(rowHtml, imagePattern);
                if (imageMatch.Success)
                {
                    payment.ImageUrl = imageMatch.Groups[1].Value;
                }

                // 필수 데이터가 있는지 확인
                if (!string.IsNullOrEmpty(payment.Date) && !string.IsNullOrEmpty(payment.ProductName))
                {
                    Console.WriteLine($"Successfully parsed payment: {payment.Date} - {payment.ProductName} - {payment.FormattedAmount}");
                    return payment;
                }
                else
                {
                    Console.WriteLine($"Missing required data: Date='{payment.Date}', Product='{payment.ProductName}'");
                }
            }
            catch (Exception ex)
            {
                // 파싱 실패 시 무시
                Console.WriteLine($"Failed to parse payment row: {ex.Message}");
            }

            return null!;
        }
    }
}