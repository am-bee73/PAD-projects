//
//  MainView.swift
//  GoRestAPI
//
//  Created by vladikkk on 28/11/2020.
//

import SwiftUI

struct MainView: View {
    @ObservedObject private var productsService = ProductsWebService.shared
    
    var body: some View {
        List {
            ForEach(self.productsService.products, id: \.id) { product in
                HStack(alignment: .center, spacing: 5) {
                    RemoteImage(name: product.name, url: product.image)
                        .cornerRadius(10)
                        .overlay(RoundedRectangle(cornerRadius: 10).stroke(Color.white, lineWidth: 1))
                        .shadow(radius: 10)
                    
                    VStack(alignment: .center, spacing: 50) {
                        Text(product.name)
                            .font(.title3)
                            .foregroundColor(.white)
                            .padding(.horizontal)
                        
                        Text("Price:  \(String(format: "%.0f", Double(product.price) ?? 0)) $")
                            .foregroundColor(.white)
                            .padding()
                    }
                }
                .background(Color.black)
                .padding()
            }
            .onDelete(perform: self.productsService.deleteProduct(atIndex:))
        }
        .padding(.vertical)
        .onAppear {
            self.productsService.getProducts()
        }
    }
}

struct MainView_Previews: PreviewProvider {
    static var previews: some View {
        MainView()
    }
}
