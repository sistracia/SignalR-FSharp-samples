package com.example.androidkotlinclient

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.material3.TextField
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.saveable.rememberSaveable
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.example.androidkotlinclient.ui.theme.AndroidKotlinClientTheme
import com.microsoft.signalr.HubConnection
import com.microsoft.signalr.HubConnectionBuilder


class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        var hubConnection: HubConnection = HubConnectionBuilder.create("YOUR URL HERE").build()

        setContent {
            AndroidKotlinClientTheme {
                var messages by rememberSaveable {
                    mutableStateOf<MutableList<Message>>(
                        mutableListOf()
                    )
                }

                LaunchedEffect(Unit) {
                    hubConnection.on("Send", { message ->
                        runOnUiThread {
                            messages.add(Message(message))
                        }
                    }, String::class.java)
                }

                // A surface container using the 'background' color from the theme
                Surface(
                    modifier = Modifier.fillMaxSize(), color = MaterialTheme.colorScheme.background
                ) {
                    ChatList(messages = messages, onSendMessage = { message ->
                        try {
                            hubConnection.send("Send", message)
                        } catch (e: Exception) {
                            e.printStackTrace()
                        }
                    })
                }
            }
        }
    }
}

@Composable
fun Chat(message: Message, modifier: Modifier = Modifier) {
    Text(
        text = message.body,
        maxLines = Int.MAX_VALUE,
        modifier = modifier
            .fillMaxWidth()
            .padding(all = 2.dp)
            .border(
                border = BorderStroke(1.dp, Color.Gray), shape = RoundedCornerShape(10)
            )
            .padding(
                vertical = 2.dp, horizontal = 4.dp
            )
    )
}

@Composable
fun ChatList(
    messages: List<Message>, onSendMessage: (txt: String) -> Unit, modifier: Modifier = Modifier
) {
    Column(modifier = modifier.padding(all = 8.dp)) {
        LazyColumn(modifier = Modifier.weight(1f)) {
            items(messages) { message ->
                Chat(message)
            }
        }

        Row(
            modifier = Modifier
                .fillMaxWidth()
                .height(IntrinsicSize.Min),
            verticalAlignment = Alignment.CenterVertically
        ) {
            var text by rememberSaveable { mutableStateOf("") }

            TextField(value = text,
                modifier = Modifier.weight(1f),
                onValueChange = { text = it },
                label = { Text("Message") })

            Spacer(modifier = Modifier.width(8.dp))

            Button(modifier = Modifier.fillMaxHeight(), shape = RoundedCornerShape(10), onClick = {
                onSendMessage(text)
                text = ""
            }) {
                Text("Send")
            }
        }
    }
}

@Preview(showBackground = true, widthDp = 480, heightDp = 720)
@Composable
fun ChatListPreview() {
    AndroidKotlinClientTheme {
        ChatList(messages = SampleData.messagesSample, onSendMessage = {})
    }
}

object SampleData {
    // Sample conversation data
    val messagesSample = listOf(
        Message(
            "First Message"
        ),
        Message(
            "Second Message"
        ),
        Message(
            "Third Message"
        ),
        Message(
            "Fourth Message"
        ),
        Message(
            "Fifth Message"
        ),
    )
}

data class Message(val body: String)