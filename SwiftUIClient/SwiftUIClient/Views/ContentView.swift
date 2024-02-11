//
//  ContentView.swift
//  SwiftUIClient
//
//  Created by Fikry Fahrezy on 10/02/24.
//

import SwiftUI
import SignalRClient

struct ContentView: View {
    @State private var connection = HubConnectionBuilder(url: URL(string: "YOUR HUB URL")!)
        .withLogging(minLogLevel: .error)
        .build()
    @State private var messages: [Message] = []
    
    var body: some View {
        ChatList(messages: messages, sendMessage: { message in
            connection.send(method: "Send", message)
        })
        .task {
            connection.on(method: "Send", callback: { (message: String) in
                messages.append(Message(body: message))
            })
            connection.start()
        }
    }
}

#Preview {
    ContentView()
}
