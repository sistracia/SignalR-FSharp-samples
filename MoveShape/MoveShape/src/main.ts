import "./style.css";
import { setupApp } from "./app.ts";

document.querySelector<HTMLDivElement>("#app")!.innerHTML = `
<div id="shape" draggable="true"></div>
`;

setupApp();
