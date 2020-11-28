//
//  GenericResponse.swift
//  GoRestAPI
//
//  Created by vladikkk on 28/11/2020.
//

import Foundation

class GenericResponse: Decodable {
    // MARK: Properties
    var code: Int?
    var meta: MetaParam?
}

class MetaParam: Decodable {
    // MARK: Properties
    var pagination: PaginationParam?
}

class PaginationParam: Decodable {
    // MARK: Properties
    var total, pages, page, limit: Int?
}
