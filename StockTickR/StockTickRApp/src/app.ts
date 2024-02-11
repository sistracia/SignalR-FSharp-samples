import * as signalR from "@microsoft/signalr";

const hubConnection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/stocks")
  .configureLogging(signalR.LogLevel.Information)
  .build();

function run() {
  const getAllStocksBtn = document.getElementById("get-all-stocks-btn")!;
  const openMarketBtn = document.getElementById("open-market-btn")!;
  const closeMarketBtn = document.getElementById("close-market-btn")!;
  const resetBtn = document.getElementById("reset-btn")!;
  const getMarketStateBtn = document.getElementById("get-market-state-btn")!;

  getAllStocksBtn.addEventListener("click", function () {
    hubConnection.invoke("GetAllStocks").then(function (res) {
      console.log(res);
    });
  });

  openMarketBtn.addEventListener("click", function () {
    hubConnection.invoke("OpenMarket");
  });

  closeMarketBtn.addEventListener("click", function () {
    hubConnection.invoke("CloseMarket");
  });

  resetBtn.addEventListener("click", function () {
    hubConnection.invoke("Reset");
  });

  getMarketStateBtn.addEventListener("click", function () {
    hubConnection.invoke("GetMarketState").then(function (res) {
      console.log(res);
    });
  });
}

export function setupApp() {
  hubConnection.start().then(run);
}
