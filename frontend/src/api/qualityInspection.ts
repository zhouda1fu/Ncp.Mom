import api from './index'
import type { PaginationRequest, PaginationResponse } from '@/types'

// 质检单状态
export enum QualityInspectionStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2
}

// 质检单类型定义
export interface QualityInspection {
  id: string
  inspectionNumber: string
  workOrderId: string
  sampleQuantity: number
  qualifiedQuantity: number
  unqualifiedQuantity: number
  status: QualityInspectionStatus
  remark?: string
  updateTime: string
}

// 质检单查询请求
export interface QualityInspectionQueryRequest extends PaginationRequest {
  workOrderId?: string
  status?: QualityInspectionStatus
  keyword?: string
}

// 创建质检单请求
export interface CreateQualityInspectionRequest {
  inspectionNumber: string
  workOrderId: string
  sampleQuantity: number
}

// 执行质检请求
export interface InspectQualityRequest {
  id: string
  qualifiedQuantity: number
  unqualifiedQuantity: number
  remark?: string
}

// 质检单API
export const qualityInspectionApi = {
  // 获取质检单列表
  getQualityInspections: (params: QualityInspectionQueryRequest) =>
    api.get<PaginationResponse<QualityInspection>>('/quality-inspections', { params }),

  // 获取质检单详情
  getQualityInspection: (id: string) =>
    api.get<QualityInspection>(`/quality-inspections/${id}`),

  // 创建质检单
  createQualityInspection: (data: CreateQualityInspectionRequest) =>
    api.post<{ id: string }>('/quality-inspections', data),

  // 执行质检
  inspectQuality: (data: InspectQualityRequest) =>
    api.post(`/quality-inspections/${data.id}/inspect`, {
      qualifiedQuantity: data.qualifiedQuantity,
      unqualifiedQuantity: data.unqualifiedQuantity,
      remark: data.remark
    })
}

