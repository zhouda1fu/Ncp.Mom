<template>
  <div class="management-page">
    <div class="page-header">
      <div class="header-content">
        <div class="title-section">
          <h1>设备管理</h1>
          <p class="subtitle">管理系统中的所有设备信息</p>
        </div>
        <el-button 
          type="primary" 
          size="large"
          class="create-btn"
          @click="showCreateDialog"
          icon="Plus"
        >
          新建设备
        </el-button>
      </div>
    </div>

    <div class="main-content">
      <div class="search-section">
        <div class="section-header">
          <h2>
            <el-icon class="section-icon"><Search /></el-icon>
            搜索筛选
          </h2>
        </div>
        <div class="search-wrapper">
          <el-form :inline="true" :model="searchParams" class="search-form">
            <el-form-item label="搜索">
              <el-input
                v-model="searchParams.keyword"
                placeholder="请输入设备编码或名称"
                clearable
                class="search-input"
                :prefix-icon="Search"
                @keyup.enter="handleSearch"
              />
            </el-form-item>
            <el-form-item label="状态">
              <el-select v-model="searchParams.status" placeholder="请选择状态" clearable>
                <el-option label="空闲" :value="0" />
                <el-option label="运行中" :value="1" />
                <el-option label="维护中" :value="2" />
                <el-option label="故障" :value="3" />
              </el-select>
            </el-form-item>
            <el-form-item label="设备类型">
              <el-select v-model="searchParams.equipmentType" placeholder="请选择设备类型" clearable>
                <el-option label="机器设备" :value="0" />
                <el-option label="工具" :value="1" />
                <el-option label="夹具" :value="2" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" class="search-btn" @click="handleSearch" icon="Search">
                搜索 
              </el-button>
            </el-form-item>
          </el-form>
        </div>
      </div>

      <div class="table-section">
        <el-table :data="equipments" v-loading="loading" stripe>
          <el-table-column prop="equipmentCode" label="设备编码" min-width="150" />
          <el-table-column prop="equipmentName" label="设备名称" min-width="200" />
          <el-table-column label="设备类型" width="120" align="center">
            <template #default="{ row }">
              {{ getEquipmentTypeText(row.equipmentType) }}
            </template>
          </el-table-column>
          <el-table-column prop="workCenterId" label="工作中心ID" width="150" />
          <el-table-column label="状态" width="100" align="center">
            <template #default="{ row }">
              <el-tag :type="getStatusType(row.status)">
                {{ getStatusText(row.status) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="currentWorkOrderId" label="当前工单ID" width="150" />
          <el-table-column label="操作" width="300" fixed="right" align="center">
            <template #default="{ row }">
              <el-button size="small" type="info" @click="handleView(row)" :icon="View">查看</el-button>
              <el-button 
                v-if="row.status === 0" 
                size="small" 
                type="warning" 
                @click="handleStartMaintenance(row)" 
                :icon="Tools"
              >
                维护
              </el-button>
              <el-button 
                v-if="row.status === 2" 
                size="small" 
                type="success" 
                @click="handleCompleteMaintenance(row)" 
                :icon="Check"
              >
                完成维护
              </el-button>
              <el-button 
                v-if="row.status === 1" 
                size="small" 
                type="primary" 
                @click="handleRelease(row)" 
                :icon="Unlock"
              >
                释放
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <div class="pagination-wrapper">
          <el-pagination
            v-model:current-page="pagination.pageIndex"
            v-model:page-size="pagination.pageSize"
            :total="pagination.total"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handlePageSizeChange"
            @current-change="handlePageCurrentChange"
          />
        </div>
      </div>
    </div>

    <!-- 创建对话框 -->
    <el-dialog
      v-model="dialogVisible"
      title="新建设备"
      width="500px"
    >
      <el-form :model="formData" :rules="formRules" ref="formRef" label-width="120px">
        <el-form-item label="设备编码" prop="equipmentCode">
          <el-input v-model="formData.equipmentCode" placeholder="请输入设备编码" />
        </el-form-item>
        <el-form-item label="设备名称" prop="equipmentName">
          <el-input v-model="formData.equipmentName" placeholder="请输入设备名称" />
        </el-form-item>
        <el-form-item label="设备类型" prop="equipmentType">
          <el-select v-model="formData.equipmentType" placeholder="请选择设备类型" style="width: 100%">
            <el-option label="机器设备" :value="0" />
            <el-option label="工具" :value="1" />
            <el-option label="夹具" :value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="工作中心">
          <el-select v-model="formData.workCenterId" placeholder="请选择工作中心（可选）" filterable clearable style="width: 100%">
            <el-option
              v-for="wc in workCenters"
              :key="wc.id"
              :label="`${wc.workCenterCode} - ${wc.workCenterName}`"
              :value="wc.id"
            />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">确定</el-button>
      </template>
    </el-dialog>

    <!-- 查看详情对话框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="设备详情"
      width="800px"
    >
      <div v-if="currentEquipment">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="设备编码">{{ currentEquipment.equipmentCode }}</el-descriptions-item>
          <el-descriptions-item label="设备名称">{{ currentEquipment.equipmentName }}</el-descriptions-item>
          <el-descriptions-item label="设备类型">{{ getEquipmentTypeText(currentEquipment.equipmentType) }}</el-descriptions-item>
          <el-descriptions-item label="工作中心ID">{{ currentEquipment.workCenterId || '-' }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="getStatusType(currentEquipment.status)">
              {{ getStatusText(currentEquipment.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="当前工单ID">{{ currentEquipment.currentWorkOrderId || '-' }}</el-descriptions-item>
          <el-descriptions-item label="更新时间">{{ currentEquipment.updateTime }}</el-descriptions-item>
        </el-descriptions>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, View, Tools, Check, Unlock } from '@element-plus/icons-vue'
import { equipmentApi, type Equipment, type CreateEquipmentRequest, EquipmentStatus, EquipmentType } from '@/api/equipment'
import { workCenterApi, type WorkCenter } from '@/api/workCenter'

const loading = ref(false)
const equipments = ref<Equipment[]>([])
const workCenters = ref<WorkCenter[]>([])
const dialogVisible = ref(false)
const detailDialogVisible = ref(false)
const formRef = ref()
const currentEquipment = ref<Equipment | null>(null)

const searchParams = ref({
  keyword: '',
  status: undefined as EquipmentStatus | undefined,
  equipmentType: undefined as EquipmentType | undefined
})

const pagination = ref({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

const formData = ref({
  equipmentCode: '',
  equipmentName: '',
  equipmentType: 0 as EquipmentType,
  workCenterId: undefined as string | undefined
})

const formRules = {
  equipmentCode: [{ required: true, message: '请输入设备编码', trigger: 'blur' }],
  equipmentName: [{ required: true, message: '请输入设备名称', trigger: 'blur' }],
  equipmentType: [{ required: true, message: '请选择设备类型', trigger: 'change' }]
}

const getStatusText = (status: EquipmentStatus) => {
  const statusMap = {
    [EquipmentStatus.Idle]: '空闲',
    [EquipmentStatus.Running]: '运行中',
    [EquipmentStatus.Maintenance]: '维护中',
    [EquipmentStatus.Fault]: '故障'
  }
  return statusMap[status] || '未知'
}

const getStatusType = (status: EquipmentStatus) => {
  const typeMap = {
    [EquipmentStatus.Idle]: 'success',
    [EquipmentStatus.Running]: 'warning',
    [EquipmentStatus.Maintenance]: 'info',
    [EquipmentStatus.Fault]: 'danger'
  }
  return typeMap[status] || 'info'
}

const getEquipmentTypeText = (type: EquipmentType) => {
  const typeMap = {
    [EquipmentType.Machine]: '机器设备',
    [EquipmentType.Tool]: '工具',
    [EquipmentType.Fixture]: '夹具'
  }
  return typeMap[type] || '未知'
}

const loadEquipments = async () => {
  loading.value = true
  try {
    const res = await equipmentApi.getEquipments({
      ...searchParams.value,
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize
    })
    equipments.value = res.data.items || []
    pagination.value.total = res.data.total || 0
  } catch (error) {
    ElMessage.error('加载设备列表失败')
  } finally {
    loading.value = false
  }
}

const loadWorkCenters = async () => {
  try {
    const res = await workCenterApi.getWorkCenters({ pageIndex: 1, pageSize: 1000 })
    workCenters.value = res.data.items || []
  } catch (error) {
    ElMessage.error('加载工作中心列表失败')
  }
}

const handleSearch = () => {
  pagination.value.pageIndex = 1
  loadEquipments()
}

const handlePageSizeChange = () => {
  loadEquipments()
}

const handlePageCurrentChange = () => {
  loadEquipments()
}

const showCreateDialog = () => {
  formData.value = {
    equipmentCode: '',
    equipmentName: '',
    equipmentType: 0,
    workCenterId: undefined
  }
  dialogVisible.value = true
}

const handleView = async (row: Equipment) => {
  try {
    const res = await equipmentApi.getEquipment(row.id)
    currentEquipment.value = res.data
    detailDialogVisible.value = true
  } catch (error) {
    ElMessage.error('加载设备详情失败')
  }
}

const handleStartMaintenance = async (row: Equipment) => {
  try {
    await ElMessageBox.confirm('确定要开始维护该设备吗？', '提示', {
      type: 'warning'
    })
    await equipmentApi.startMaintenance(row.id)
    ElMessage.success('开始维护成功')
    loadEquipments()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('开始维护失败')
    }
  }
}

const handleCompleteMaintenance = async (row: Equipment) => {
  try {
    await ElMessageBox.confirm('确定要完成维护该设备吗？', '提示', {
      type: 'warning'
    })
    await equipmentApi.completeMaintenance(row.id)
    ElMessage.success('完成维护成功')
    loadEquipments()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('完成维护失败')
    }
  }
}

const handleRelease = async (row: Equipment) => {
  try {
    await ElMessageBox.confirm('确定要释放该设备吗？', '提示', {
      type: 'warning'
    })
    await equipmentApi.releaseEquipment(row.id)
    ElMessage.success('释放成功')
    loadEquipments()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('释放失败')
    }
  }
}

const handleSubmit = async () => {
  await formRef.value.validate()
  try {
    await equipmentApi.createEquipment(formData.value as CreateEquipmentRequest)
    ElMessage.success('创建成功')
    dialogVisible.value = false
    loadEquipments()
  } catch (error) {
    ElMessage.error('创建失败')
  }
}

onMounted(() => {
  loadEquipments()
  loadWorkCenters()
})
</script>

<style scoped>
.management-page {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.title-section h1 {
  margin: 0;
  font-size: 24px;
}

.subtitle {
  margin: 5px 0 0 0;
  color: #666;
}

.search-section {
  background: #fff;
  padding: 20px;
  border-radius: 4px;
  margin-bottom: 20px;
}

.table-section {
  background: #fff;
  padding: 20px;
  border-radius: 4px;
}

.pagination-wrapper {
  margin-top: 20px;
  text-align: right;
}
</style>

