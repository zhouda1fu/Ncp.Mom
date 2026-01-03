import api from './index'
import type { PaginationRequest, PaginationResponse } from '@/types'

// 生产计划状态
export enum ProductionPlanStatus {
  Draft = 0,
  Approved = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4
}

// 生产计划类型定义
export interface ProductionPlan {
  id: string
  planNumber: string
  productId: string
  quantity: number
  status: ProductionPlanStatus
  plannedStartDate: string
  plannedEndDate: string
  updateTime: string
}

// 生产计划查询请求
export interface ProductionPlanQueryRequest extends PaginationRequest {
  status?: ProductionPlanStatus
}

// 创建生产计划请求
export interface CreateProductionPlanRequest {
  planNumber: string
  productId: string
  quantity: number
  plannedStartDate: string
  plannedEndDate: string
}

// 生产计划API
export const productionPlanApi = {
  // 获取生产计划列表
  getProductionPlans: (params: ProductionPlanQueryRequest) =>
    api.get<PaginationResponse<ProductionPlan>>('/production-plans', { params }),

  // 获取生产计划详情
  getProductionPlan: (id: string) =>
    api.get<ProductionPlan>(`/production-plans/${id}`),

  // 创建生产计划
  createProductionPlan: (data: CreateProductionPlanRequest) =>
    api.post<{ id: string }>('/production-plans', data),

  // 审批生产计划
  approveProductionPlan: (id: string) =>
    api.post(`/production-plans/${id}/approve`, {}),

  // 启动生产计划
  startProductionPlan: (id: string) =>
    api.post(`/production-plans/${id}/start`, {}),

  // 完成生产计划
  completeProductionPlan: (id: string) =>
    api.post<boolean>(`/production-plans/${id}/complete`, {}),

  // 取消生产计划
  cancelProductionPlan: (id: string) =>
    api.post<boolean>(`/production-plans/${id}/cancel`, {}),

  // 生成工单
  generateWorkOrders: (id: string) =>
    api.post<{ productionPlanId: string; workOrderIds: string[] }>(`/production-plans/${id}/generate-work-orders`)
}

