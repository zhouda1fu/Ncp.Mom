import api from './index'
import type { PaginationRequest, PaginationResponse } from '@/types'

// 工艺路线类型定义
export interface Routing {
  id: string
  routingNumber: string
  name: string
  productId: string
  operationCount: number
}

// 工艺路线详情
export interface RoutingDetail {
  id: string
  routingNumber: string
  name: string
  productId: string
  operations: RoutingOperation[]
}

// 工艺路线工序
export interface RoutingOperation {
  sequence: number
  operationName: string
  workCenterId: string
  standardTime: number
}

// 工艺路线查询请求
export interface RoutingQueryRequest extends PaginationRequest {
  keyword?: string
  productId?: string
}

// 创建工艺路线请求
export interface CreateRoutingRequest {
  routingNumber: string
  name: string
  productId: string
}

// 更新工艺路线请求
export interface UpdateRoutingRequest {
  routingId: string
  routingNumber: string
  name: string
}

// 添加工序请求
export interface AddRoutingOperationRequest {
  routingId: string
  sequence: number
  operationName: string
  workCenterId: string
  standardTime: number
}

// 工艺路线API
export const routingApi = {
  // 获取工艺路线列表
  getRoutings: (params: RoutingQueryRequest) =>
    api.get<PaginationResponse<Routing>>('/routings', { params }),

  // 获取工艺路线详情
  getRouting: (id: string) =>
    api.get<RoutingDetail>(`/routings/${id}`),

  // 创建工艺路线
  createRouting: (data: CreateRoutingRequest) =>
    api.post<{ id: string }>('/routings', data),

  // 更新工艺路线
  updateRouting: (data: UpdateRoutingRequest) =>
    api.put<{ routingId: string }>(`/routings/${data.routingId}`, data),

  // 删除工艺路线
  deleteRouting: (id: string) =>
    api.delete<boolean>(`/routings/${id}`),

  // 添加工序
  addOperation: (data: AddRoutingOperationRequest) =>
    api.post(`/routings/${data.routingId}/operations`, data),

  // 删除工序
  removeOperation: (routingId: string, sequence: number) =>
    api.delete<boolean>(`/routings/${routingId}/operations/${sequence}`)
}

