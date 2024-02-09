/**
 * The original example implementation is using jQuery,
 * (See https://github.com/aspnet/SignalR-samples/blob/main/MoveShape/MoveShape/wwwroot/index.html)
 * but in this example we use plain JavaScript
 * (Ref: https://www.w3schools.com/howto/howto_js_draggable.asp)
 */
import * as signalR from "@microsoft/signalr";

export function setupApp() {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/shapeHub")
    .build();

  const shape = document.getElementById("shape") as HTMLElement;

  let pos1 = 0;
  let pos2 = 0;
  let pos3 = 0;
  let pos4 = 0;

  function moveShape(x: number, y: number) {
    shape.style.top = x + "px";
    shape.style.left = y + "px";
  }

  function onMouseUp() {
    // stop moving when mouse button is released:
    document.onmouseup = null;
    document.onmousemove = null;
  }

  function onMouseMove(e: MouseEvent) {
    e.preventDefault();

    // calculate the new cursor position:
    pos1 = pos3 - e.clientX;
    pos2 = pos4 - e.clientY;
    pos3 = e.clientX;
    pos4 = e.clientY;

    // set the element's new position:
    const x = shape.offsetTop - pos2;
    const y = shape.offsetLeft - pos1;
    moveShape(x, y);
    connection.invoke("MoveShape", x, y || 0);
  }

  function onMouseDown(e: MouseEvent) {
    e.preventDefault();

    // get the mouse cursor position at startup:
    pos3 = e.clientX;
    pos4 = e.clientY;
    document.onmouseup = onMouseUp;

    // call a function whenever the cursor moves:
    document.onmousemove = onMouseMove;
  }

  connection.on("shapeMoved", function (x, y) {
    moveShape(x, y);
  });

  connection
    .start()
    .then(function () {
      shape.addEventListener("mousedown", onMouseDown);
    })
    .catch(function (reason) {
      console.log(reason);
    });
}
