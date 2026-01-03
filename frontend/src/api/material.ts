import api from './index'
import type { PaginationRequest, PaginationResponse } from '@/types'

// 物料类型定义
export interface Material {
  id: string
  materialCode: string
  materialName: string
  specification?: string
  unit: string
  updateTime: string
}

// 物料查询请求
export interface MaterialQueryRequest extends PaginationRequest {
  keyword?: string
}

// 创建物料请求
export interface CreateMaterialRequest {
  materialCode: string
  materialName: string
  specification?: string
  unit?: string
}

// 更新物料请求
export interface UpdateMaterialRequest {
  id: string
  materialCode: string
  materialName: string
  specification?: string
  unit?: string
}

// 物料API
export const materialApi = {
  // 获取物料列表
  getMaterials: (params: MaterialQueryRequest) =>
    api.get<PaginationResponse<Material>>('/materials', { params }),

  // 获取物料详情
  getMaterial: (id: string) =>
    api.get<Material>(`/materials/${id}`),

  // 创建物料
  createMaterial: (data: CreateMaterialRequest) =>
    api.post<{ id: string }>('/materials', data),

  // 更新物料
  updateMaterial: (data: UpdateMaterialRequest) =>
    api.put(`/materials/${data.id}`, data),

  // 删除物料
  deleteMaterial: (id: string) =>
    api.delete<boolean>(`/materials/${id}`)
}

