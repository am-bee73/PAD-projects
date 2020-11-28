//
//  RemoteImage.swift
//  GoRestAPI
//
//  Created by vladikkk on 28/11/2020.
//

import SwiftUI

struct RemoteImage: View {
    private enum LoadState {
        case loading, success, failure
    }
    
    private class Loader: ObservableObject {
        var data = UIImage()
        var state = LoadState.loading
                
        init(name: String, url: String) {
            guard let parsedURL = URL(string: url) else {
                fatalError("Invalid URL: \(url)")
            }
            
            if let image = ProductsImageCache.get(forKey: name) {
                self.data = image
                self.state = .success
                
                DispatchQueue.main.async {
                    self.objectWillChange.send()
                }
            } else {
                DispatchQueue.global().async {
                    URLSession.shared.dataTask(with: parsedURL) { data, response, error in
                        if let data = data, let image = UIImage(data: data) {
                            self.data = image
                            self.state = .success
                        } else {
                            self.state = .failure
                        }
                        
                        DispatchQueue.main.async {
                            self.objectWillChange.send()
                        }
                    }.resume()
                }
            }
        }
    }
    
    @StateObject private var loader: Loader
    var loading: Image
    var failure: Image
    
    var body: some View {
        selectImage()
            .resizable()
            .frame(width: 150, height: 150)
    }
    
    init(name: String, url: String, loading: Image = Image(systemName: "photo"), failure: Image = Image(systemName: "multiply.circle")) {
        _loader = StateObject(wrappedValue: Loader(name: name, url: url))
        self.loading = loading
        self.failure = failure
    }
    
    private func selectImage() -> Image {
        switch loader.state {
        case .loading:
            return loading
        case .failure:
            return failure
        default:
            return Image(uiImage: loader.data)
        }
    }
}
