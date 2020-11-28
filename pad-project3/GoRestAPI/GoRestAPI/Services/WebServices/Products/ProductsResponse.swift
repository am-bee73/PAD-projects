//
//  ProductsResponse.swift
//  GoRestAPI
//
//  Created by vladikkk on 28/11/2020.
//

import Foundation

// MARK: - RequestData
struct RequestData: Codable {
    let code: Int
    let meta: Meta
    let data: [ProductsResponseData]
}

// MARK: - Datum
struct ProductsResponseData: Codable {
    let id: Int
    let name, datumDescription: String
    let image: String
    let price, discountAmount: String
    let status: Bool
    let categories: [Category]

    enum CodingKeys: String, CodingKey {
        case id, name
        case datumDescription = "description"
        case image, price
        case discountAmount = "discount_amount"
        case status, categories
    }
}

// MARK: - Category
struct Category: Codable {
    let id: Int
    let name: String
}

// MARK: - Meta
struct Meta: Codable {
    let pagination: Pagination
}

// MARK: - Pagination
struct Pagination: Codable {
    let total, pages, page, limit: Int
}
