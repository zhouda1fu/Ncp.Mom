import api from './index'
import type { PaginationRequest, PaginationResponse } from '@/types'

// 产品类型定义
export interface Product {
  id: string
  productCode: string
  productName: string
}

// 产品查询请求
export interface ProductQueryRequest extends PaginationRequest {
  keyword?: string
}

// 创建产品请求
export interface CreateProductRequest {
  productCode: string
  productName: string
}

// 更新产品请求
export interface UpdateProductRequest {
  productId: string
  productCode: string
  productName: string
}

// 产品API
export const productApi = {
  // 获取产品列表
  getProducts: (params: ProductQueryRequest) =>
    api.get<PaginationResponse<Product>>('/products', { params }),

  // 获取产品详情
  getProduct: (id: string) =>
    api.get<Product>(`/products/${id}`),

  // 创建产品
  createProduct: (data: CreateProductRequest) =>
    api.post<{ productId: string; productCode: string; productName: string }>('/products', data),

  // 更新产品
  updateProduct: (data: UpdateProductRequest) =>
    api.put<{ productId: string }>(`/products/${data.productId}`, data),

  // 删除产品
  deleteProduct: (id: string) =>
    api.delete<boolean>(`/products/${id}`)
}

