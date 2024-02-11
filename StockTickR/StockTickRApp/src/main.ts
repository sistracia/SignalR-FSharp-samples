import { setupApp } from "./app.ts";

document.querySelector<HTMLDivElement>("#app")!.innerHTML = `
  <div>
    <button id="get-all-stocks-btn">Get All Stocks</button>
    <button id="open-market-btn">Open Market</button>
    <button id="close-market-btn">Close Market</button>
    <button id="reset-btn">Reset</button>
    <button id="get-market-state-btn">Get Market State</button>
  </div>
`;

setupApp();
