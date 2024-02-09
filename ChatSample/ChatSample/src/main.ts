import "./style.css";
import { setupApp } from "./app.ts";

document.querySelector<HTMLDivElement>("#app")!.innerHTML = `
    <div class="container">
        <input type="text" id="message" />
        <input type="button" id="sendmessage" value="Send" />
        <ul id="discussion"></ul>
    </div>
`;

setupApp();
