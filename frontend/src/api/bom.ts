import api from './index'
import type { PaginationRequest, PaginationResponse } from '@/types'

// BOM类型定义
export interface BomItem {
  id: string
  materialId: string
  quantity: number
  unit: string
}

export interface Bom {
  id: string
  bomNumber: string
  productId: string
  version: number
  isActive: boolean
  items: BomItem[]
  updateTime: string
}

// BOM查询请求
export interface BomQueryRequest extends PaginationRequest {
  productId?: string
  isActive?: boolean
  keyword?: string
}

// 创建BOM请求
export interface CreateBomRequest {
  bomNumber: string
  productId: string
  version: number
}

// 添加BOM项请求
export interface AddBomItemRequest {
  id: string
  materialId: string
  quantity: number
  unit: string
}

// BOM API
export const bomApi = {
  // 获取BOM列表
  getBoms: (params: BomQueryRequest) =>
    api.get<PaginationResponse<Bom>>('/boms', { params }),

  // 获取BOM详情
  getBom: (id: string) =>
    api.get<Bom>(`/boms/${id}`),

  // 创建BOM
  createBom: (data: CreateBomRequest) =>
    api.post<{ id: string }>('/boms', data),

  // 添加BOM项
  addBomItem: (data: AddBomItemRequest) =>
    api.post(`/boms/${data.id}/items`, {
      materialId: data.materialId,
      quantity: data.quantity,
      unit: data.unit
    }),

  // 删除BOM项
  removeBomItem: (bomId: string, itemId: string) =>
    api.delete(`/boms/${bomId}/items/${itemId}`),

  // 停用BOM
  deactivateBom: (id: string) =>
    api.post(`/boms/${id}/deactivate`)
}

