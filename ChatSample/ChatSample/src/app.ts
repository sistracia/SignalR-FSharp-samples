import * as signalR from "@microsoft/signalr";

export function setupApp() {
  document.addEventListener("DOMContentLoaded", function () {
    const messageInput = document.getElementById("message") as HTMLInputElement;
    const sendmessageBtn = document.getElementById(
      "sendmessage"
    ) as HTMLButtonElement;
    const discussionContainer = document.getElementById(
      "discussion"
    ) as HTMLLIElement;

    // Get the user name and store it to prepend to messages.
    const name = prompt("Enter your name:", "");

    // Set initial focus to message input box.
    messageInput.focus();

    // Start the connection.
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/chat")
      .build();

    // Create a function that the hub can call to broadcast messages.
    connection.on("broadcastMessage", function (name, message) {
      // Html encode display name and message.
      const encodedName = name;
      const encodedMsg = message;

      // Add the message to the page.
      const liElement = document.createElement("li");
      liElement.innerHTML =
        "<strong>" + encodedName + "</strong>:&nbsp;&nbsp;" + encodedMsg;
      discussionContainer.appendChild(liElement);
    });

    // Transport fallback functionality is now built into start.
    connection
      .start()
      .then(function () {
        console.log("connection started");
        sendmessageBtn.addEventListener("click", function (event) {
          // Call the Send method on the hub.
          connection.invoke("send", name, messageInput.value);

          // Clear text box and reset focus for next comment.
          messageInput.value = "";
          messageInput.focus();
          event.preventDefault();
        });
      })
      .catch((error) => {
        console.error(error.message);
      });
  });
}
