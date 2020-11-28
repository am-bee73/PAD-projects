//
//  ProductsWebService.swift
//  GoRestAPI
//
//  Created by vladikkk on 28/11/2020.
//

import Foundation
import SwiftUI

class ProductsWebService: ObservableObject {
    // MARK: Properties
    // Perform requests
    private let genericRequests = GenericHTTPRequest()
    
    // URL string
    private var URLString = "https://gorest.co.in/public-api/products"
    
    // Store response data
    @Published var products = [ProductsResponseData]()
    
    // Singleton
    static let shared = ProductsWebService()
    
    // MARK: Initializers
    private init() {}
    
    // MARK: Methods
    func getProducts() {
        self.genericRequests.HTTPGetRequest(stringURL: self.URLString) { (respData) in
            guard let data = respData else {
                return
            }
            
            // Decode data
            let decoder = JSONDecoder()
            guard let responseData = try? decoder.decode(RequestData.self, from: data) else {
                NSLog("Error while decoding: \(#line)")
                return
            }
            
            for product in responseData.data {
                guard let url = URL(string: product.image) else {
                    NSLog("")
                    return
                }
                
                DispatchQueue.global().async {
                    URLSession.shared.dataTask(with: url) { data, response, error in
                        if let data = data, let image = UIImage(data: data) {
                            ProductsImageCache.set(forKey: product.name, image: image)
                            NSLog("Image added to cache: \(#line)")
                        } else {
                            NSLog("Failed to cache image: \(#line)")
                        }
                        
                        DispatchQueue.main.async {
                            self.objectWillChange.send()
                        }
                    }.resume()
                }
            }
            
            DispatchQueue.main.async {
                self.products = responseData.data
            }
        }
    }
    
    func deleteProduct(atIndex index: IndexSet) {
        var tempProducts = self.products
        
        tempProducts.remove(atOffsets: index)
        
        DispatchQueue.main.async {
            self.products = tempProducts
        }
    }
}

class ProductsImageCache {
    static var cache = NSCache<NSString, UIImage>()
    
    static func get(forKey: String) -> UIImage? {
        return cache.object(forKey: NSString(string: forKey))
    }
    
    static func set(forKey: String, image: UIImage) {
        cache.setObject(image, forKey: NSString(string: forKey))
    }
}
