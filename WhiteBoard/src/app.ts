import * as signalR from "@microsoft/signalr";

export function setupApp() {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5001/draw")
    .build();

  const canvas = document.getElementById("canvas") as HTMLCanvasElement;
  const colorSelect = document.getElementById("color") as HTMLSelectElement;
  const output = document.getElementById("output") as HTMLDivElement;

  const ctx = canvas.getContext("2d");

  const canvasX = canvas.offsetLeft;
  const canvasY = canvas.offsetTop;

  let lastMouseX = 0;
  let lastMouseY = 0;
  let mouseX = 0;
  let mouseY = 0;
  let isMouseDown = false;

  function drawCanvas(
    prevX: number,
    prevY: number,
    x: number,
    y: number,
    color: string
  ) {
    if (ctx === null) {
      return;
    }

    ctx.beginPath();
    ctx.globalCompositeOperation = "source-over";
    ctx.strokeStyle = color;
    ctx.lineWidth = 3;
    ctx.moveTo(prevX, prevY);
    ctx.lineTo(x, y);
    ctx.lineJoin = ctx.lineCap = "round";
    ctx.stroke();
  }

  function clearMousePositions() {
    lastMouseX = 0;
    lastMouseY = 0;
  }

  function onMouseMove(e: MouseEvent) {
    mouseX = parseInt(String(e.clientX - canvasX));
    mouseY = parseInt(String(e.clientY - canvasY));

    const color = colorSelect.value;

    if (lastMouseX > 0 && lastMouseY > 0 && isMouseDown) {
      drawCanvas(mouseX, mouseY, lastMouseX, lastMouseY, color);
      connection.invoke("draw", lastMouseX, lastMouseY, mouseX, mouseY, color);
    }

    lastMouseX = mouseX;
    lastMouseY = mouseY;

    output.innerHTML = `current: ${mouseX}, ${mouseY}<br/>last: ${lastMouseX}, ${lastMouseY}<br/>mousedown: ${isMouseDown}`;
  }

  function onMouseDown(e: MouseEvent) {
    mouseX = parseInt(String(e.clientX - canvasX));
    lastMouseX = mouseX;
    mouseY = parseInt(String(e.clientY - canvasY));
    lastMouseY = mouseY;
    isMouseDown = true;
  }

  function onMouseUp() {
    isMouseDown = false;
  }

  canvas.addEventListener("mouseout", clearMousePositions);
  canvas.addEventListener("mousemove", onMouseMove);
  canvas.addEventListener("mousedown", onMouseDown);
  canvas.addEventListener("mouseup", onMouseUp);

  connection.on("draw", function (prevX, prevY, x, y, color) {
    drawCanvas(prevX, prevY, x, y, color);
  });

  connection.start().catch(function (reason) {
    console.log(reason);
  });
}
