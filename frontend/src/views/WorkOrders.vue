<template>
  <div class="management-page">
    <div class="page-header">
      <div class="header-content">
        <div class="title-section">
          <h1>工单管理</h1>
          <p class="subtitle">管理系统中的所有工单信息</p>
        </div>
        <el-button 
          type="primary" 
          size="large"
          class="create-btn"
          @click="showCreateDialog"
          icon="Plus"
        >
          新建工单
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
                placeholder="请输入工单编号"
                clearable
                class="search-input"
                :prefix-icon="Search"
                @keyup.enter="handleSearch"
              />
            </el-form-item>
            <el-form-item label="生产计划">
              <el-select v-model="searchParams.productionPlanId" placeholder="请选择生产计划" clearable filterable>
                <el-option
                  v-for="plan in productionPlans"
                  :key="plan.id"
                  :label="plan.planNumber"
                  :value="plan.id"
                />
              </el-select>
            </el-form-item>
            <el-form-item label="状态">
              <el-select v-model="searchParams.status" placeholder="请选择状态" clearable>
                <el-option label="已创建" :value="0" />
                <el-option label="进行中" :value="1" />
                <el-option label="已暂停" :value="2" />
                <el-option label="已完成" :value="3" />
                <el-option label="已取消" :value="4" />
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
        <el-table :data="workOrders" v-loading="loading" stripe>
          <el-table-column prop="workOrderNumber" label="工单编号" min-width="150" />
          <el-table-column prop="productionPlanId" label="生产计划ID" min-width="150" />
          <el-table-column prop="productId" label="产品ID" min-width="120" />
          <el-table-column prop="quantity" label="数量" width="100" align="center" />
          <el-table-column label="完成进度" width="150" align="center">
            <template #default="{ row }">
              <el-progress 
                :percentage="Math.round((row.completedQuantity / row.quantity) * 100)" 
                :status="row.status === 3 ? 'success' : undefined"
              />
              <span style="font-size: 12px; color: #666;">
                {{ row.completedQuantity }} / {{ row.quantity }}
              </span>
            </template>
          </el-table-column>
          <el-table-column label="状态" width="100" align="center">
            <template #default="{ row }">
              <el-tag :type="getStatusType(row.status)">
                {{ getStatusText(row.status) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="startTime" label="开始时间" width="160" />
          <el-table-column prop="endTime" label="结束时间" width="160" />
          <el-table-column label="操作" width="350" fixed="right" align="center">
            <template #default="{ row }">
              <el-button size="small" type="info" @click="handleView(row)" :icon="View">查看</el-button>
              <el-button 
                v-if="row.status === 0 || row.status === 2" 
                size="small" 
                type="primary" 
                @click="handleStart(row)" 
                :icon="VideoPlay"
              >
                启动
              </el-button>
              <el-button 
                v-if="row.status === 1" 
                size="small" 
                type="warning" 
                @click="handlePause(row)" 
                :icon="VideoPause"
              >
                暂停
              </el-button>
              <el-button 
                v-if="row.status === 2" 
                size="small" 
                type="success" 
                @click="handleResume(row)" 
                :icon="VideoPlay"
              >
                恢复
              </el-button>
              <el-button 
                v-if="row.status === 1" 
                size="small" 
                type="success" 
                @click="showReportDialog(row)" 
                :icon="Edit"
              >
                报工
              </el-button>
              <el-button 
                v-if="row.status !== 3 && row.status !== 4" 
                size="small" 
                type="danger" 
                @click="handleCancel(row)" 
                :icon="Close"
              >
                取消
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
      title="新建工单"
      width="600px"
    >
      <el-form :model="formData" :rules="formRules" ref="formRef" label-width="120px">
        <el-form-item label="工单编号" prop="workOrderNumber">
          <el-input v-model="formData.workOrderNumber" placeholder="请输入工单编号" />
        </el-form-item>
        <el-form-item label="生产计划" prop="productionPlanId">
          <el-select v-model="formData.productionPlanId" placeholder="请选择生产计划" filterable style="width: 100%">
            <el-option
              v-for="plan in productionPlans"
              :key="plan.id"
              :label="plan.planNumber"
              :value="plan.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="产品" prop="productId">
          <el-select v-model="formData.productId" placeholder="请选择产品" filterable style="width: 100%">
            <el-option
              v-for="product in products"
              :key="product.id"
              :label="`${product.productCode} - ${product.productName}`"
              :value="product.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="数量" prop="quantity">
          <el-input-number v-model="formData.quantity" :min="1" style="width: 100%" />
        </el-form-item>
        <el-form-item label="工艺路线" prop="routingId">
          <el-select v-model="formData.routingId" placeholder="请选择工艺路线" filterable style="width: 100%">
            <el-option
              v-for="routing in routings"
              :key="routing.id"
              :label="`${routing.routingNumber} - ${routing.name}`"
              :value="routing.id"
            />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">确定</el-button>
      </template>
    </el-dialog>

    <!-- 报工对话框 -->
    <el-dialog
      v-model="reportDialogVisible"
      title="工单报工"
      width="500px"
    >
      <el-form :model="reportForm" :rules="reportRules" ref="reportFormRef" label-width="120px">
        <el-form-item label="工单编号">
          <el-input :value="currentWorkOrder?.workOrderNumber" disabled />
        </el-form-item>
        <el-form-item label="已完成数量">
          <el-input :value="currentWorkOrder?.completedQuantity" disabled />
        </el-form-item>
        <el-form-item label="总数量">
          <el-input :value="currentWorkOrder?.quantity" disabled />
        </el-form-item>
        <el-form-item label="本次报工数量" prop="quantity">
          <el-input-number 
            v-model="reportForm.quantity" 
            :min="1" 
            :max="(currentWorkOrder?.quantity || 0) - (currentWorkOrder?.completedQuantity || 0)"
            style="width: 100%" 
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="reportDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleReportSubmit">确定</el-button>
      </template>
    </el-dialog>

    <!-- 查看详情对话框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="工单详情"
      width="800px"
    >
      <div v-if="currentWorkOrder">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="工单编号">{{ currentWorkOrder.workOrderNumber }}</el-descriptions-item>
          <el-descriptions-item label="生产计划ID">{{ currentWorkOrder.productionPlanId }}</el-descriptions-item>
          <el-descriptions-item label="产品ID">{{ currentWorkOrder.productId }}</el-descriptions-item>
          <el-descriptions-item label="工艺路线ID">{{ currentWorkOrder.routingId }}</el-descriptions-item>
          <el-descriptions-item label="数量">{{ currentWorkOrder.quantity }}</el-descriptions-item>
          <el-descriptions-item label="已完成数量">{{ currentWorkOrder.completedQuantity }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="getStatusType(currentWorkOrder.status)">
              {{ getStatusText(currentWorkOrder.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="开始时间">{{ currentWorkOrder.startTime || '-' }}</el-descriptions-item>
          <el-descriptions-item label="结束时间">{{ currentWorkOrder.endTime || '-' }}</el-descriptions-item>
          <el-descriptions-item label="更新时间">{{ currentWorkOrder.updateTime }}</el-descriptions-item>
        </el-descriptions>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, View, VideoPlay, VideoPause, Edit, Close } from '@element-plus/icons-vue'
import { workOrderApi, type WorkOrder, type CreateWorkOrderRequest, type ReportProgressRequest, WorkOrderStatus } from '@/api/workOrder'
import { productionPlanApi, type ProductionPlan } from '@/api/productionPlan'
import { productApi, type Product } from '@/api/product'
import { routingApi, type Routing } from '@/api/routing'

const loading = ref(false)
const workOrders = ref<WorkOrder[]>([])
const productionPlans = ref<ProductionPlan[]>([])
const products = ref<Product[]>([])
const routings = ref<Routing[]>([])
const dialogVisible = ref(false)
const reportDialogVisible = ref(false)
const detailDialogVisible = ref(false)
const formRef = ref()
const reportFormRef = ref()
const currentWorkOrder = ref<WorkOrder | null>(null)

const searchParams = ref({
  keyword: '',
  productionPlanId: undefined as string | undefined,
  status: undefined as WorkOrderStatus | undefined
})

const pagination = ref({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

const formData = ref({
  workOrderNumber: '',
  productionPlanId: '',
  productId: '',
  quantity: 1,
  routingId: ''
})

const reportForm = ref({
  quantity: 1
})

const formRules = {
  workOrderNumber: [{ required: true, message: '请输入工单编号', trigger: 'blur' }],
  productionPlanId: [{ required: true, message: '请选择生产计划', trigger: 'change' }],
  productId: [{ required: true, message: '请选择产品', trigger: 'change' }],
  quantity: [{ required: true, message: '请输入数量', trigger: 'blur' }],
  routingId: [{ required: true, message: '请选择工艺路线', trigger: 'change' }]
}

const reportRules = {
  quantity: [{ required: true, message: '请输入报工数量', trigger: 'blur' }]
}

const getStatusText = (status: WorkOrderStatus) => {
  const statusMap = {
    [WorkOrderStatus.Created]: '已创建',
    [WorkOrderStatus.InProgress]: '进行中',
    [WorkOrderStatus.Paused]: '已暂停',
    [WorkOrderStatus.Completed]: '已完成',
    [WorkOrderStatus.Cancelled]: '已取消'
  }
  return statusMap[status] || '未知'
}

const getStatusType = (status: WorkOrderStatus) => {
  const typeMap = {
    [WorkOrderStatus.Created]: 'info',
    [WorkOrderStatus.InProgress]: 'warning',
    [WorkOrderStatus.Paused]: 'warning',
    [WorkOrderStatus.Completed]: 'success',
    [WorkOrderStatus.Cancelled]: 'danger'
  }
  return typeMap[status] || 'info'
}

const loadWorkOrders = async () => {
  loading.value = true
  try {
    const res = await workOrderApi.getWorkOrders({
      ...searchParams.value,
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize
    })
    workOrders.value = res.data.items || []
    pagination.value.total = res.data.total || 0
  } catch (error) {
    ElMessage.error('加载工单列表失败')
  } finally {
    loading.value = false
  }
}

const loadProductionPlans = async () => {
  try {
    const res = await productionPlanApi.getProductionPlans({ pageIndex: 1, pageSize: 1000 })
    productionPlans.value = res.data.items || []
  } catch (error) {
    ElMessage.error('加载生产计划列表失败')
  }
}

const loadProducts = async () => {
  try {
    const res = await productApi.getProducts({ pageIndex: 1, pageSize: 1000 })
    products.value = res.data.items || []
  } catch (error) {
    ElMessage.error('加载产品列表失败')
  }
}

const loadRoutings = async () => {
  try {
    const res = await routingApi.getRoutings({ pageIndex: 1, pageSize: 1000 })
    routings.value = res.data.items || []
  } catch (error) {
    ElMessage.error('加载工艺路线列表失败')
  }
}

const handleSearch = () => {
  pagination.value.pageIndex = 1
  loadWorkOrders()
}

const handlePageSizeChange = () => {
  loadWorkOrders()
}

const handlePageCurrentChange = () => {
  loadWorkOrders()
}

const showCreateDialog = () => {
  formData.value = {
    workOrderNumber: '',
    productionPlanId: '',
    productId: '',
    quantity: 1,
    routingId: ''
  }
  dialogVisible.value = true
}

const handleView = async (row: WorkOrder) => {
  try {
    const res = await workOrderApi.getWorkOrder(row.id)
    currentWorkOrder.value = res.data
    detailDialogVisible.value = true
  } catch (error) {
    ElMessage.error('加载工单详情失败')
  }
}

const handleStart = async (row: WorkOrder) => {
  try {
    await ElMessageBox.confirm('确定要启动该工单吗？', '提示', {
      type: 'warning'
    })
    await workOrderApi.startWorkOrder(row.id)
    ElMessage.success('启动成功')
    loadWorkOrders()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('启动失败')
    }
  }
}

const handlePause = async (row: WorkOrder) => {
  try {
    await ElMessageBox.confirm('确定要暂停该工单吗？', '提示', {
      type: 'warning'
    })
    await workOrderApi.pauseWorkOrder(row.id)
    ElMessage.success('暂停成功')
    loadWorkOrders()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('暂停失败')
    }
  }
}

const handleResume = async (row: WorkOrder) => {
  try {
    await ElMessageBox.confirm('确定要恢复该工单吗？', '提示', {
      type: 'warning'
    })
    await workOrderApi.resumeWorkOrder(row.id)
    ElMessage.success('恢复成功')
    loadWorkOrders()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('恢复失败')
    }
  }
}

const handleCancel = async (row: WorkOrder) => {
  try {
    await ElMessageBox.confirm('确定要取消该工单吗？', '提示', {
      type: 'warning'
    })
    await workOrderApi.cancelWorkOrder(row.id)
    ElMessage.success('取消成功')
    loadWorkOrders()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('取消失败')
    }
  }
}

const showReportDialog = (row: WorkOrder) => {
  currentWorkOrder.value = row
  reportForm.value = {
    quantity: 1
  }
  reportDialogVisible.value = true
}

const handleReportSubmit = async () => {
  if (!currentWorkOrder.value) return
  
  await reportFormRef.value.validate()
  try {
    await workOrderApi.reportProgress({
      workOrderId: currentWorkOrder.value.id,
      quantity: reportForm.value.quantity
    })
    ElMessage.success('报工成功')
    reportDialogVisible.value = false
    loadWorkOrders()
  } catch (error) {
    ElMessage.error('报工失败')
  }
}

const handleSubmit = async () => {
  await formRef.value.validate()
  try {
    await workOrderApi.createWorkOrder(formData.value as CreateWorkOrderRequest)
    ElMessage.success('创建成功')
    dialogVisible.value = false
    loadWorkOrders()
  } catch (error) {
    ElMessage.error('创建失败')
  }
}

onMounted(() => {
  loadWorkOrders()
  loadProductionPlans()
  loadProducts()
  loadRoutings()
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

