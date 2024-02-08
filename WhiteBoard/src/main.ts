import "./style.css";
import { setupApp } from "./app.ts";

document.querySelector<HTMLDivElement>("#app")!.innerHTML = `
    <canvas
        id="canvas"
        width="800"
        height="500"
    ></canvas>
    <select id="color">
        <option value="black">Black</option>
        <option value="red">Red</option>
        <option value="yellow">Yellow</option>
        <option value="green">Green</option>
        <option value="blue">Blue</option>
    </select>
    <div id="output"></div>
`;

setupApp();
