//
//  Message.swift
//  SwiftUIClient
//
//  Created by Fikry Fahrezy on 10/02/24.
//

import Foundation

struct Message: Identifiable {
    var id: UUID
    var body: String
    
    init(id: UUID = UUID(), body: String) {
        self.id = id
        self.body = body
    }
}


extension Message {
    static let sampleMessages: [Message] = Array(repeating: Message(body: "Message"), count: 1000)
}
