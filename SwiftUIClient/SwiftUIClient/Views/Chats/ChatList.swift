//
//  ChatList.swift
//  SwiftUIClient
//
//  Created by Fikry Fahrezy on 11/02/24.
//

import SwiftUI

struct ChatList: View {
    let messages: [Message]
    let sendMessage: (_ message: String) -> Void
    
    @State private var newMessage: String = ""
    
    var body: some View {
        VStack {
            List {
                ForEach(messages) { message in
                    ChatRow(message: message)
                        .listRowSeparator(.hidden)
                }
            }
            .listStyle(.plain)
            .padding()
            
            HStack {
                TextField("Message", text: $newMessage)
                    .textFieldStyle(RoundedBorderTextFieldStyle())
                    .padding()
                
                Button {
                    sendMessage(newMessage)
                    newMessage = ""
                } label: {
                    Text("Send")
                }
                .padding()
            }
        }
    }
}

#Preview {
    ChatList(messages: Message.sampleMessages, sendMessage: { message in})
}
