<template>
  <div class="management-page">
    <div class="page-header">
      <div class="header-content">
        <div class="title-section">
          <h1>生产计划管理</h1>
          <p class="subtitle">管理系统中的所有生产计划信息</p>
        </div>
        <el-button 
          type="primary" 
          size="large"
          class="create-btn"
          @click="showCreateDialog"
          icon="Plus"
        >
          新建生产计划
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
                placeholder="请输入生产计划编号"
                clearable
                class="search-input"
                :prefix-icon="Search"
                @keyup.enter="handleSearch"
              />
            </el-form-item>
            <el-form-item label="状态">
              <el-select v-model="searchParams.status" placeholder="请选择状态" clearable>
                <el-option label="草稿" :value="0" />
                <el-option label="已审批" :value="1" />
                <el-option label="进行中" :value="2" />
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
        <el-table :data="productionPlans" v-loading="loading" stripe>
          <el-table-column prop="planNumber" label="计划编号" min-width="150" />
          <el-table-column prop="productId" label="产品ID" min-width="120" />
          <el-table-column prop="quantity" label="数量" width="100" align="center" />
          <el-table-column label="状态" width="100" align="center">
            <template #default="{ row }">
              <el-tag :type="getStatusType(row.status)">
                {{ getStatusText(row.status) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="plannedStartDate" label="计划开始日期" width="120" />
          <el-table-column prop="plannedEndDate" label="计划结束日期" width="120" />
          <el-table-column label="操作" width="300" fixed="right" align="center">
            <template #default="{ row }">
              <el-button size="small" type="info" @click="handleView(row)" :icon="View">查看</el-button>
              <el-button 
                v-if="row.status === 0" 
                size="small" 
                type="success" 
                @click="handleApprove(row)" 
                :icon="Check"
              >
                审批
              </el-button>
              <el-button 
                v-if="row.status === 1" 
                size="small" 
                type="primary" 
                @click="handleStart(row)" 
                :icon="VideoPlay"
              >
                启动
              </el-button>
              <el-button 
                v-if="row.status === 1 || row.status === 0" 
                size="small" 
                type="warning" 
                @click="handleGenerateWorkOrders(row)" 
                :icon="DocumentAdd"
              >
                生成工单
              </el-button>
              <el-button 
                v-if="row.status === 2" 
                size="small" 
                type="success" 
                @click="handleComplete(row)" 
                :icon="Check"
              >
                完成
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
      title="新建生产计划"
      width="600px"
    >
      <el-form :model="formData" :rules="formRules" ref="formRef" label-width="120px">
        <el-form-item label="计划编号" prop="planNumber">
          <el-input v-model="formData.planNumber" placeholder="请输入计划编号" />
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
        <el-form-item label="计划开始日期" prop="plannedStartDate">
          <el-date-picker
            v-model="formData.plannedStartDate"
            type="datetime"
            placeholder="请选择计划开始日期"
            style="width: 100%"
            value-format="YYYY-MM-DDTHH:mm:ss"
          />
        </el-form-item>
        <el-form-item label="计划结束日期" prop="plannedEndDate">
          <el-date-picker
            v-model="formData.plannedEndDate"
            type="datetime"
            placeholder="请选择计划结束日期"
            style="width: 100%"
            value-format="YYYY-MM-DDTHH:mm:ss"
          />
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
      title="生产计划详情"
      width="800px"
    >
      <div v-if="currentPlan">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="计划编号">{{ currentPlan.planNumber }}</el-descriptions-item>
          <el-descriptions-item label="产品ID">{{ currentPlan.productId }}</el-descriptions-item>
          <el-descriptions-item label="数量">{{ currentPlan.quantity }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="getStatusType(currentPlan.status)">
              {{ getStatusText(currentPlan.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="计划开始日期">{{ currentPlan.plannedStartDate }}</el-descriptions-item>
          <el-descriptions-item label="计划结束日期">{{ currentPlan.plannedEndDate }}</el-descriptions-item>
          <el-descriptions-item label="更新时间">{{ currentPlan.updateTime }}</el-descriptions-item>
        </el-descriptions>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, View, Check, VideoPlay, DocumentAdd, Close } from '@element-plus/icons-vue'
import { productionPlanApi, type ProductionPlan, type CreateProductionPlanRequest, ProductionPlanStatus } from '@/api/productionPlan'
import { productApi, type Product } from '@/api/product'

const loading = ref(false)
const productionPlans = ref<ProductionPlan[]>([])
const products = ref<Product[]>([])
const dialogVisible = ref(false)
const detailDialogVisible = ref(false)
const formRef = ref()
const currentPlan = ref<ProductionPlan | null>(null)

const searchParams = ref({
  keyword: '',
  status: undefined as ProductionPlanStatus | undefined
})

const pagination = ref({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

const formData = ref({
  planNumber: '',
  productId: '',
  quantity: 1,
  plannedStartDate: '',
  plannedEndDate: ''
})

const formRules = {
  planNumber: [{ required: true, message: '请输入计划编号', trigger: 'blur' }],
  productId: [{ required: true, message: '请选择产品', trigger: 'change' }],
  quantity: [{ required: true, message: '请输入数量', trigger: 'blur' }],
  plannedStartDate: [{ required: true, message: '请选择计划开始日期', trigger: 'change' }],
  plannedEndDate: [{ required: true, message: '请选择计划结束日期', trigger: 'change' }]
}

const getStatusText = (status: ProductionPlanStatus) => {
  const statusMap = {
    [ProductionPlanStatus.Draft]: '草稿',
    [ProductionPlanStatus.Approved]: '已审批',
    [ProductionPlanStatus.InProgress]: '进行中',
    [ProductionPlanStatus.Completed]: '已完成',
    [ProductionPlanStatus.Cancelled]: '已取消'
  }
  return statusMap[status] || '未知'
}

const getStatusType = (status: ProductionPlanStatus) => {
  const typeMap = {
    [ProductionPlanStatus.Draft]: 'info',
    [ProductionPlanStatus.Approved]: 'success',
    [ProductionPlanStatus.InProgress]: 'warning',
    [ProductionPlanStatus.Completed]: 'success',
    [ProductionPlanStatus.Cancelled]: 'danger'
  }
  return typeMap[status] || 'info'
}

const loadProductionPlans = async () => {
  loading.value = true
  try {
    const res = await productionPlanApi.getProductionPlans({
      ...searchParams.value,
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize
    })
    productionPlans.value = res.data.items || []
    pagination.value.total = res.data.total || 0
  } catch (error) {
    ElMessage.error('加载生产计划列表失败')
  } finally {
    loading.value = false
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

const handleSearch = () => {
  pagination.value.pageIndex = 1
  loadProductionPlans()
}

const handlePageSizeChange = () => {
  loadProductionPlans()
}

const handlePageCurrentChange = () => {
  loadProductionPlans()
}

const showCreateDialog = () => {
  formData.value = {
    planNumber: '',
    productId: '',
    quantity: 1,
    plannedStartDate: '',
    plannedEndDate: ''
  }
  dialogVisible.value = true
}

const handleView = async (row: ProductionPlan) => {
  try {
    const res = await productionPlanApi.getProductionPlan(row.id)
    currentPlan.value = res.data
    detailDialogVisible.value = true
  } catch (error) {
    ElMessage.error('加载生产计划详情失败')
  }
}

const handleApprove = async (row: ProductionPlan) => {
  try {
    await ElMessageBox.confirm('确定要审批该生产计划吗？', '提示', {
      type: 'warning'
    })
    await productionPlanApi.approveProductionPlan(row.id)
    ElMessage.success('审批成功')
    loadProductionPlans()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('审批失败')
    }
  }
}

const handleStart = async (row: ProductionPlan) => {
  try {
    await ElMessageBox.confirm('确定要启动该生产计划吗？', '提示', {
      type: 'warning'
    })
    await productionPlanApi.startProductionPlan(row.id)
    ElMessage.success('启动成功')
    loadProductionPlans()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('启动失败')
    }
  }
}

const handleComplete = async (row: ProductionPlan) => {
  try {
    await ElMessageBox.confirm('确定要完成该生产计划吗？', '提示', {
      type: 'warning'
    })
    await productionPlanApi.completeProductionPlan(row.id)
    ElMessage.success('完成成功')
    loadProductionPlans()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('完成失败')
    }
  }
}

const handleCancel = async (row: ProductionPlan) => {
  try {
    await ElMessageBox.confirm('确定要取消该生产计划吗？', '提示', {
      type: 'warning'
    })
    await productionPlanApi.cancelProductionPlan(row.id)
    ElMessage.success('取消成功')
    loadProductionPlans()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('取消失败')
    }
  }
}

const handleGenerateWorkOrders = async (row: ProductionPlan) => {
  try {
    await ElMessageBox.confirm('确定要生成工单吗？', '提示', {
      type: 'warning'
    })
    const res = await productionPlanApi.generateWorkOrders(row.id)
    ElMessage.success(`成功生成 ${res.data.workOrderIds.length} 个工单`)
    loadProductionPlans()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('生成工单失败')
    }
  }
}

const handleSubmit = async () => {
  await formRef.value.validate()
  try {
    await productionPlanApi.createProductionPlan(formData.value as CreateProductionPlanRequest)
    ElMessage.success('创建成功')
    dialogVisible.value = false
    loadProductionPlans()
  } catch (error) {
    ElMessage.error('创建失败')
  }
}

onMounted(() => {
  loadProductionPlans()
  loadProducts()
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

