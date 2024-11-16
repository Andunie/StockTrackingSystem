using AspireBorsa.ApiService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using HtmlAgilityPack;
using System.Globalization;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AspireBorsa.ApiService.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public StockController(HttpClient httpClient, AppDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStocks()
        {
            var url = "https://bigpara.hurriyet.com.tr/borsa/canli-borsa/";
            var response = await _httpClient.GetStringAsync(url);

            var stocks = ParseStockData(response);

            // Veri tabanı kayıt işlemi:
            await SaveStocksToDatabase(stocks);

            return Ok(stocks);
        }


        [HttpGet]
        //[Route("/GetStocksById/{symbol}")]
        public async Task<List<Stock>> GetStocksWithId(string symbol)
        {
            if (!string.IsNullOrEmpty(symbol))
            {
               return await _context.Stocks.Where(_ => _.Symbol == symbol).ToListAsync();
            }

            return new List<Stock>();
        }

        private List<Stock> ParseStockData(string html)
        {
            var stocks = new List<Stock>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var rows = htmlDoc.DocumentNode.SelectNodes("//ul[contains(@class, 'live-stock-item')]");
            foreach (var row in rows)
            {
                var cells = row.SelectNodes("li");

                var stock = new Stock();
                stock.Symbol = cells[0].InnerText.Trim().Replace("&nbsp;","").Trim();
                stock.LastPrice = Convert.ToDouble(cells[1].InnerText.Trim());
                stock.BuyPrice = Convert.ToDouble(cells[2].InnerText.Trim());
                stock.SellPrice = Convert.ToDouble(cells[3].InnerText.Trim());
                stock.HighPrice = Convert.ToDouble(cells[4].InnerText.Trim());
                stock.LowPrice = Convert.ToDouble(cells[5].InnerText.Trim());
                stock.AveragePrice = Convert.ToDouble(cells[6].InnerText.Trim());   
                stock.PercentageChange = Convert.ToDouble(cells[7].InnerText.Trim());
                stock.VolumeLot = Convert.ToDouble(cells[12].InnerText.Trim().Replace(".",""));
                stock.VolumeTL = long.Parse(cells[13].InnerText.Trim().Replace(".", ""));
                stock.LastTransactionTime = ParseDateTimeSafely(cells[14].InnerText.Trim());
                stock.isActive = true;

                stocks.Add(stock);
            }

            return stocks;
        }

        private DateTime ParseDateTimeSafely(string input)
        {
            if (DateTime.TryParseExact(input, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
            return DateTime.MinValue;   
        }

        private async Task SaveStocksToDatabase(List<Stock> stocks)
        {
            // Tüm eski kayıtlar false yapıldı.
            var allStocks = await _context.Stocks.ToListAsync();
            foreach (var stock in allStocks)
            {
                stock.isActive = false;
            }
            await _context.SaveChangesAsync();

            // Yeni veriler: 
            _context.Stocks.AddRange(stocks);
            await _context.SaveChangesAsync();
        }
    }
}
