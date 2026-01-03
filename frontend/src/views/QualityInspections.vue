<template>
  <div class="management-page">
    <div class="page-header">
      <div class="header-content">
        <div class="title-section">
          <h1>质量管理</h1>
          <p class="subtitle">管理系统中的所有质检单信息</p>
        </div>
        <el-button 
          type="primary" 
          size="large"
          class="create-btn"
          @click="showCreateDialog"
          icon="Plus"
        >
          新建质检单
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
                placeholder="请输入质检单编号"
                clearable
                class="search-input"
                :prefix-icon="Search"
                @keyup.enter="handleSearch"
              />
            </el-form-item>
            <el-form-item label="工单">
              <el-select v-model="searchParams.workOrderId" placeholder="请选择工单" clearable filterable>
                <el-option
                  v-for="order in workOrders"
                  :key="order.id"
                  :label="order.workOrderNumber"
                  :value="order.id"
                />
              </el-select>
            </el-form-item>
            <el-form-item label="状态">
              <el-select v-model="searchParams.status" placeholder="请选择状态" clearable>
                <el-option label="待检验" :value="0" />
                <el-option label="检验中" :value="1" />
                <el-option label="已完成" :value="2" />
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
        <el-table :data="qualityInspections" v-loading="loading" stripe>
          <el-table-column prop="inspectionNumber" label="质检单编号" min-width="150" />
          <el-table-column prop="workOrderId" label="工单ID" min-width="150" />
          <el-table-column prop="sampleQuantity" label="抽样数量" width="100" align="center" />
          <el-table-column prop="qualifiedQuantity" label="合格数量" width="100" align="center" />
          <el-table-column prop="unqualifiedQuantity" label="不合格数量" width="120" align="center" />
          <el-table-column label="状态" width="100" align="center">
            <template #default="{ row }">
              <el-tag :type="getStatusType(row.status)">
                {{ getStatusText(row.status) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="updateTime" label="更新时间" width="160" />
          <el-table-column label="操作" width="200" fixed="right" align="center">
            <template #default="{ row }">
              <el-button size="small" type="info" @click="handleView(row)" :icon="View">查看</el-button>
              <el-button 
                v-if="row.status === 0 || row.status === 1" 
                size="small" 
                type="primary" 
                @click="showInspectDialog(row)" 
                :icon="Edit"
              >
                质检
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
      title="新建质检单"
      width="500px"
    >
      <el-form :model="formData" :rules="formRules" ref="formRef" label-width="120px">
        <el-form-item label="质检单编号" prop="inspectionNumber">
          <el-input v-model="formData.inspectionNumber" placeholder="请输入质检单编号" />
        </el-form-item>
        <el-form-item label="工单" prop="workOrderId">
          <el-select v-model="formData.workOrderId" placeholder="请选择工单" filterable style="width: 100%">
            <el-option
              v-for="order in workOrders"
              :key="order.id"
              :label="order.workOrderNumber"
              :value="order.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="抽样数量" prop="sampleQuantity">
          <el-input-number v-model="formData.sampleQuantity" :min="1" style="width: 100%" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">确定</el-button>
      </template>
    </el-dialog>

    <!-- 质检对话框 -->
    <el-dialog
      v-model="inspectDialogVisible"
      title="执行质检"
      width="500px"
    >
      <el-form :model="inspectForm" :rules="inspectRules" ref="inspectFormRef" label-width="120px">
        <el-form-item label="质检单编号">
          <el-input :value="currentInspection?.inspectionNumber" disabled />
        </el-form-item>
        <el-form-item label="抽样数量">
          <el-input :value="currentInspection?.sampleQuantity" disabled />
        </el-form-item>
        <el-form-item label="合格数量" prop="qualifiedQuantity">
          <el-input-number 
            v-model="inspectForm.qualifiedQuantity" 
            :min="0" 
            :max="currentInspection?.sampleQuantity || 0"
            style="width: 100%" 
          />
        </el-form-item>
        <el-form-item label="不合格数量" prop="unqualifiedQuantity">
          <el-input-number 
            v-model="inspectForm.unqualifiedQuantity" 
            :min="0" 
            :max="currentInspection?.sampleQuantity || 0"
            style="width: 100%" 
          />
        </el-form-item>
        <el-form-item label="备注">
          <el-input 
            v-model="inspectForm.remark" 
            type="textarea" 
            :rows="3"
            placeholder="请输入备注信息"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="inspectDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleInspectSubmit">确定</el-button>
      </template>
    </el-dialog>

    <!-- 查看详情对话框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="质检单详情"
      width="800px"
    >
      <div v-if="currentInspection">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="质检单编号">{{ currentInspection.inspectionNumber }}</el-descriptions-item>
          <el-descriptions-item label="工单ID">{{ currentInspection.workOrderId }}</el-descriptions-item>
          <el-descriptions-item label="抽样数量">{{ currentInspection.sampleQuantity }}</el-descriptions-item>
          <el-descriptions-item label="合格数量">{{ currentInspection.qualifiedQuantity }}</el-descriptions-item>
          <el-descriptions-item label="不合格数量">{{ currentInspection.unqualifiedQuantity }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="getStatusType(currentInspection.status)">
              {{ getStatusText(currentInspection.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="备注" :span="2">{{ currentInspection.remark || '-' }}</el-descriptions-item>
          <el-descriptions-item label="更新时间">{{ currentInspection.updateTime }}</el-descriptions-item>
        </el-descriptions>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, View, Edit } from '@element-plus/icons-vue'
import { qualityInspectionApi, type QualityInspection, type CreateQualityInspectionRequest, type InspectQualityRequest, QualityInspectionStatus } from '@/api/qualityInspection'
import { workOrderApi, type WorkOrder } from '@/api/workOrder'

const loading = ref(false)
const qualityInspections = ref<QualityInspection[]>([])
const workOrders = ref<WorkOrder[]>([])
const dialogVisible = ref(false)
const inspectDialogVisible = ref(false)
const detailDialogVisible = ref(false)
const formRef = ref()
const inspectFormRef = ref()
const currentInspection = ref<QualityInspection | null>(null)

const searchParams = ref({
  keyword: '',
  workOrderId: undefined as string | undefined,
  status: undefined as QualityInspectionStatus | undefined
})

const pagination = ref({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

const formData = ref({
  inspectionNumber: '',
  workOrderId: '',
  sampleQuantity: 1
})

const inspectForm = ref({
  qualifiedQuantity: 0,
  unqualifiedQuantity: 0,
  remark: ''
})

const formRules = {
  inspectionNumber: [{ required: true, message: '请输入质检单编号', trigger: 'blur' }],
  workOrderId: [{ required: true, message: '请选择工单', trigger: 'change' }],
  sampleQuantity: [{ required: true, message: '请输入抽样数量', trigger: 'blur' }]
}

const inspectRules = {
  qualifiedQuantity: [{ required: true, message: '请输入合格数量', trigger: 'blur' }],
  unqualifiedQuantity: [{ required: true, message: '请输入不合格数量', trigger: 'blur' }]
}

const getStatusText = (status: QualityInspectionStatus) => {
  const statusMap = {
    [QualityInspectionStatus.Pending]: '待检验',
    [QualityInspectionStatus.InProgress]: '检验中',
    [QualityInspectionStatus.Completed]: '已完成'
  }
  return statusMap[status] || '未知'
}

const getStatusType = (status: QualityInspectionStatus) => {
  const typeMap = {
    [QualityInspectionStatus.Pending]: 'info',
    [QualityInspectionStatus.InProgress]: 'warning',
    [QualityInspectionStatus.Completed]: 'success'
  }
  return typeMap[status] || 'info'
}

const loadQualityInspections = async () => {
  loading.value = true
  try {
    const res = await qualityInspectionApi.getQualityInspections({
      ...searchParams.value,
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize
    })
    qualityInspections.value = res.data.items || []
    pagination.value.total = res.data.total || 0
  } catch (error) {
    ElMessage.error('加载质检单列表失败')
  } finally {
    loading.value = false
  }
}

const loadWorkOrders = async () => {
  try {
    const res = await workOrderApi.getWorkOrders({ pageIndex: 1, pageSize: 1000 })
    workOrders.value = res.data.items || []
  } catch (error) {
    ElMessage.error('加载工单列表失败')
  }
}

const handleSearch = () => {
  pagination.value.pageIndex = 1
  loadQualityInspections()
}

const handlePageSizeChange = () => {
  loadQualityInspections()
}

const handlePageCurrentChange = () => {
  loadQualityInspections()
}

const showCreateDialog = () => {
  formData.value = {
    inspectionNumber: '',
    workOrderId: '',
    sampleQuantity: 1
  }
  dialogVisible.value = true
}

const handleView = async (row: QualityInspection) => {
  try {
    const res = await qualityInspectionApi.getQualityInspection(row.id)
    currentInspection.value = res.data
    detailDialogVisible.value = true
  } catch (error) {
    ElMessage.error('加载质检单详情失败')
  }
}

const showInspectDialog = (row: QualityInspection) => {
  currentInspection.value = row
  inspectForm.value = {
    qualifiedQuantity: 0,
    unqualifiedQuantity: 0,
    remark: ''
  }
  inspectDialogVisible.value = true
}

const handleInspectSubmit = async () => {
  if (!currentInspection.value) return
  
  await inspectFormRef.value.validate()
  
  // 验证合格数量和不合格数量之和等于抽样数量
  const total = inspectForm.value.qualifiedQuantity + inspectForm.value.unqualifiedQuantity
  if (total !== currentInspection.value.sampleQuantity) {
    ElMessage.error(`合格数量和不合格数量之和必须等于抽样数量（${currentInspection.value.sampleQuantity}）`)
    return
  }
  
  try {
    await qualityInspectionApi.inspectQuality({
      id: currentInspection.value.id,
      qualifiedQuantity: inspectForm.value.qualifiedQuantity,
      unqualifiedQuantity: inspectForm.value.unqualifiedQuantity,
      remark: inspectForm.value.remark
    })
    ElMessage.success('质检成功')
    inspectDialogVisible.value = false
    loadQualityInspections()
  } catch (error) {
    ElMessage.error('质检失败')
  }
}

const handleSubmit = async () => {
  await formRef.value.validate()
  try {
    await qualityInspectionApi.createQualityInspection(formData.value as CreateQualityInspectionRequest)
    ElMessage.success('创建成功')
    dialogVisible.value = false
    loadQualityInspections()
  } catch (error) {
    ElMessage.error('创建失败')
  }
}

onMounted(() => {
  loadQualityInspections()
  loadWorkOrders()
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

