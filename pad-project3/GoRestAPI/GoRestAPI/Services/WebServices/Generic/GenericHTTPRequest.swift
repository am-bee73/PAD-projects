//
//  GenericHTTPRequest.swift
//  GoRestAPI
//
//  Created by vladikkk on 28/11/2020.
//

import Foundation

class GenericHTTPRequest {
    // MARK: Properties
    
    
    // MARK: Initializers
    
    
    // MARK: Methods
    func HTTPGetRequest(stringURL: String, completion: @escaping (Data?) -> ()) {
        guard let url = URL(string: stringURL) else {
            NSLog("Failed to create URL: \(#function)")
            return
        }
        
        URLSession.shared.dataTask(with: url) { (data, response, error) in
            if let httpResponse = response as? HTTPURLResponse, httpResponse.statusCode == 200 {
                if let error = error {
                    NSLog("Request error: \(error.localizedDescription)")
                    NSLog("Status code: \(httpResponse.statusCode)")
                    completion(nil)
                } else if let data = data {
                    NSLog("Data received: \(#line)")
                    NSLog("Status code: \(httpResponse.statusCode)")
                    completion(data)
                } else {
                    NSLog("Request error: \(#line)")
                    NSLog("Status code: \(httpResponse.statusCode)")
                    completion(nil)
                }
            } else {
                if let httpResponse = response as? HTTPURLResponse {
                    NSLog("Request error: \(#line)")
                    NSLog("Status code: \(httpResponse.statusCode)")
                    completion(nil)
                } else {
                    NSLog("Request error: \(#line)")
                    completion(nil)
                }
            }
        }.resume()
    }
}
