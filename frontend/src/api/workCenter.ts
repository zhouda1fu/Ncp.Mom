import api from './index'
import type { PaginationRequest, PaginationResponse } from '@/types'

// 工作中心类型定义
export interface WorkCenter {
  id: string
  workCenterCode: string
  workCenterName: string
}

// 工作中心查询请求
export interface WorkCenterQueryRequest extends PaginationRequest {
  keyword?: string
}

// 创建工作中心请求
export interface CreateWorkCenterRequest {
  workCenterCode: string
  workCenterName: string
}

// 更新工作中心请求
export interface UpdateWorkCenterRequest {
  workCenterId: string
  workCenterCode: string
  workCenterName: string
}

// 工作中心API
export const workCenterApi = {
  // 获取工作中心列表
  getWorkCenters: (params: WorkCenterQueryRequest) =>
    api.get<PaginationResponse<WorkCenter>>('/work-centers', { params }),

  // 获取工作中心详情
  getWorkCenter: (id: string) =>
    api.get<WorkCenter>(`/work-centers/${id}`),

  // 创建工作中心
  createWorkCenter: (data: CreateWorkCenterRequest) =>
    api.post<{ workCenterId: string; workCenterCode: string; workCenterName: string }>('/work-centers', data),

  // 更新工作中心
  updateWorkCenter: (data: UpdateWorkCenterRequest) =>
    api.put<{ workCenterId: string }>(`/work-centers/${data.workCenterId}`, data),

  // 删除工作中心
  deleteWorkCenter: (id: string) =>
    api.delete<boolean>(`/work-centers/${id}`)
}

