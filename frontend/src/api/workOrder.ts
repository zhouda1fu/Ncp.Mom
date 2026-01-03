import api from './index'
import type { PaginationRequest, PaginationResponse } from '@/types'

// 工单状态
export enum WorkOrderStatus {
  Created = 0,
  InProgress = 1,
  Paused = 2,
  Completed = 3,
  Cancelled = 4
}

// 工单类型定义
export interface WorkOrder {
  id: string
  workOrderNumber: string
  productionPlanId: string
  productId: string
  quantity: number
  completedQuantity: number
  routingId: string
  status: WorkOrderStatus
  startTime?: string
  endTime?: string
  updateTime: string
}

// 工单查询请求
export interface WorkOrderQueryRequest extends PaginationRequest {
  productionPlanId?: string
  status?: WorkOrderStatus
}

// 创建工单请求
export interface CreateWorkOrderRequest {
  workOrderNumber: string
  productionPlanId: string
  productId: string
  quantity: number
  routingId: string
}

// 报工请求
export interface ReportProgressRequest {
  workOrderId: string
  quantity: number
}

// 工单API
export const workOrderApi = {
  // 获取工单列表
  getWorkOrders: (params: WorkOrderQueryRequest) =>
    api.get<PaginationResponse<WorkOrder>>('/work-orders', { params }),

  // 获取工单详情
  getWorkOrder: (id: string) =>
    api.get<WorkOrder>(`/work-orders/${id}`),

  // 创建工单
  createWorkOrder: (data: CreateWorkOrderRequest) =>
    api.post<{ id: string }>('/work-orders', data),

  // 启动工单
  startWorkOrder: (id: string) =>
    api.post(`/work-orders/${id}/start`),

  // 暂停工单
  pauseWorkOrder: (id: string) =>
    api.post<boolean>(`/work-orders/${id}/pause`),

  // 恢复工单
  resumeWorkOrder: (id: string) =>
    api.post<boolean>(`/work-orders/${id}/resume`),

  // 取消工单
  cancelWorkOrder: (id: string) =>
    api.post<boolean>(`/work-orders/${id}/cancel`),

  // 报工
  reportProgress: (data: ReportProgressRequest) =>
    api.post(`/work-orders/${data.workOrderId}/report-progress`, { quantity: data.quantity })
}

