//
//  SignalRService.swift
//  SwiftUIClient
//
//  Created by Fikry Fahrezy on 10/02/24.
//

import Foundation
import SignalRClient

public class SignalRService {
    private var connection: HubConnection
    
    public init(url: URL) {
        connection = HubConnectionBuilder(url: url).withLogging(minLogLevel: .error).build()
        connection.on(method: "Send", callback: { (message: String) in
            self.handleMessage(message)
        })
        
        connection.start()
    }
    
    private func handleMessage(_ message: String) {
        // Do something with the message.
    }
}
