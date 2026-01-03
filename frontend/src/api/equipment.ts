import api from './index'
import type { PaginationRequest, PaginationResponse } from '@/types'

// 设备状态
export enum EquipmentStatus {
  Idle = 0,
  Running = 1,
  Maintenance = 2,
  Fault = 3
}

// 设备类型
export enum EquipmentType {
  Machine = 0,
  Tool = 1,
  Fixture = 2
}

// 设备类型定义
export interface Equipment {
  id: string
  equipmentCode: string
  equipmentName: string
  equipmentType: EquipmentType
  workCenterId?: string
  status: EquipmentStatus
  currentWorkOrderId?: string
  updateTime: string
}

// 设备查询请求
export interface EquipmentQueryRequest extends PaginationRequest {
  status?: EquipmentStatus
  equipmentType?: EquipmentType
  workCenterId?: string
  keyword?: string
}

// 创建设备请求
export interface CreateEquipmentRequest {
  equipmentCode: string
  equipmentName: string
  equipmentType: EquipmentType
  workCenterId?: string
}

// 分配设备请求
export interface AssignEquipmentRequest {
  id: string
  workOrderId: string
}

// 设备API
export const equipmentApi = {
  // 获取设备列表
  getEquipments: (params: EquipmentQueryRequest) =>
    api.get<PaginationResponse<Equipment>>('/equipments', { params }),

  // 获取设备详情
  getEquipment: (id: string) =>
    api.get<Equipment>(`/equipments/${id}`),

  // 创建设备
  createEquipment: (data: CreateEquipmentRequest) =>
    api.post<{ id: string }>('/equipments', data),

  // 分配设备
  assignEquipment: (data: AssignEquipmentRequest) =>
    api.post(`/equipments/${data.id}/assign`, { workOrderId: data.workOrderId }),

  // 释放设备
  releaseEquipment: (id: string) =>
    api.post(`/equipments/${id}/release`),

  // 开始维护
  startMaintenance: (id: string) =>
    api.post(`/equipments/${id}/start-maintenance`),

  // 完成维护
  completeMaintenance: (id: string) =>
    api.post(`/equipments/${id}/complete-maintenance`)
}

