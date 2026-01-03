<template>
  <div class="management-page">
    <div class="page-header">
      <div class="header-content">
        <div class="title-section">
          <h1>工艺路线管理</h1>
          <p class="subtitle">管理系统中的所有工艺路线信息</p>
        </div>
        <el-button 
          type="primary" 
          size="large"
          class="create-btn"
          @click="showCreateDialog"
          icon="Plus"
        >
          新建工艺路线
        </el-button>
      </div>
    </div>

    <div class="main-content">
      <div class="search-section">
        <el-form :inline="true" :model="searchParams" class="search-form">
          <el-form-item label="搜索">
            <el-input
              v-model="searchParams.keyword"
              placeholder="请输入工艺路线编码或名称"
              clearable
              @keyup.enter="handleSearch"
            />
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="handleSearch" icon="Search">搜索</el-button>
          </el-form-item>
        </el-form>
      </div>

      <div class="table-section">
        <el-table :data="routings" v-loading="loading" stripe>
          <el-table-column prop="routingNumber" label="工艺路线编码" min-width="150" />
          <el-table-column prop="name" label="工艺路线名称" min-width="200" />
          <el-table-column prop="operationCount" label="工序数量" width="100" align="center" />
          <el-table-column label="操作" width="200" fixed="right" align="center">
            <template #default="{ row }">
              <el-button size="small" type="primary" @click="handleView(row)" :icon="View">查看</el-button>
              <el-button size="small" type="warning" @click="handleEdit(row)" :icon="Edit">编辑</el-button>
              <el-button size="small" type="danger" @click="handleDelete(row)" :icon="Delete">删除</el-button>
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

    <!-- 创建/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="isEdit ? '编辑工艺路线' : '新建工艺路线'"
      width="500px"
    >
      <el-form :model="formData" :rules="formRules" ref="formRef" label-width="120px">
        <el-form-item label="工艺路线编码" prop="routingNumber">
          <el-input v-model="formData.routingNumber" placeholder="请输入工艺路线编码" />
        </el-form-item>
        <el-form-item label="工艺路线名称" prop="name">
          <el-input v-model="formData.name" placeholder="请输入工艺路线名称" />
        </el-form-item>
        <el-form-item label="产品" prop="productId">
          <el-select v-model="formData.productId" placeholder="请选择产品" filterable>
            <el-option
              v-for="product in products"
              :key="product.id"
              :label="`${product.productCode} - ${product.productName}`"
              :value="product.id"
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
      title="工艺路线详情"
      width="800px"
    >
      <div v-if="currentRouting">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="工艺路线编码">{{ currentRouting.routingNumber }}</el-descriptions-item>
          <el-descriptions-item label="工艺路线名称">{{ currentRouting.name }}</el-descriptions-item>
          <el-descriptions-item label="产品ID">{{ currentRouting.productId }}</el-descriptions-item>
          <el-descriptions-item label="工序数量">{{ currentRouting.operations?.length || 0 }}</el-descriptions-item>
        </el-descriptions>
        
        <div style="margin-top: 20px;">
          <div style="display: flex; justify-content: space-between; margin-bottom: 10px;">
            <h3>工序列表</h3>
            <el-button type="primary" size="small" @click="showAddOperationDialog" icon="Plus">添加工序</el-button>
          </div>
          <el-table :data="currentRouting.operations" stripe>
            <el-table-column prop="sequence" label="序号" width="80" align="center" />
            <el-table-column prop="operationName" label="工序名称" />
            <el-table-column prop="workCenterId" label="工作中心ID" />
            <el-table-column prop="standardTime" label="标准工时(小时)" width="120" />
            <el-table-column label="操作" width="100" align="center">
              <template #default="{ row }">
                <el-button size="small" type="danger" @click="handleDeleteOperation(row)" :icon="Delete">删除</el-button>
              </template>
            </el-table-column>
          </el-table>
        </div>
      </div>
    </el-dialog>

    <!-- 添加工序对话框 -->
    <el-dialog
      v-model="operationDialogVisible"
      title="添加工序"
      width="500px"
    >
      <el-form :model="operationForm" :rules="operationRules" ref="operationFormRef" label-width="120px">
        <el-form-item label="序号" prop="sequence">
          <el-input-number v-model="operationForm.sequence" :min="1" />
        </el-form-item>
        <el-form-item label="工序名称" prop="operationName">
          <el-input v-model="operationForm.operationName" placeholder="请输入工序名称" />
        </el-form-item>
        <el-form-item label="工作中心" prop="workCenterId">
          <el-select v-model="operationForm.workCenterId" placeholder="请选择工作中心" filterable>
            <el-option
              v-for="wc in workCenters"
              :key="wc.id"
              :label="`${wc.workCenterCode} - ${wc.workCenterName}`"
              :value="wc.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="标准工时(小时)" prop="standardTime">
          <el-input-number v-model="operationForm.standardTime" :min="0" :precision="2" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="operationDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleAddOperation">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Edit, Delete, View } from '@element-plus/icons-vue'
import { routingApi, type Routing, type RoutingDetail, type CreateRoutingRequest, type UpdateRoutingRequest, type AddRoutingOperationRequest } from '@/api/routing'
import { productApi, type Product } from '@/api/product'
import { workCenterApi, type WorkCenter } from '@/api/workCenter'

const loading = ref(false)
const routings = ref<Routing[]>([])
const products = ref<Product[]>([])
const workCenters = ref<WorkCenter[]>([])
const dialogVisible = ref(false)
const detailDialogVisible = ref(false)
const operationDialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref()
const operationFormRef = ref()
const currentRouting = ref<RoutingDetail | null>(null)

const searchParams = ref({
  keyword: ''
})

const pagination = ref({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

const formData = ref({
  routingNumber: '',
  name: '',
  productId: ''
})

const operationForm = ref({
  sequence: 1,
  operationName: '',
  workCenterId: '',
  standardTime: 0
})

const formRules = {
  routingNumber: [{ required: true, message: '请输入工艺路线编码', trigger: 'blur' }],
  name: [{ required: true, message: '请输入工艺路线名称', trigger: 'blur' }],
  productId: [{ required: true, message: '请选择产品', trigger: 'change' }]
}

const operationRules = {
  sequence: [{ required: true, message: '请输入序号', trigger: 'blur' }],
  operationName: [{ required: true, message: '请输入工序名称', trigger: 'blur' }],
  workCenterId: [{ required: true, message: '请选择工作中心', trigger: 'change' }],
  standardTime: [{ required: true, message: '请输入标准工时', trigger: 'blur' }]
}

const loadRoutings = async () => {
  loading.value = true
  try {
    const res = await routingApi.getRoutings({
      ...searchParams.value,
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize
    })
    routings.value = res.data.items || []
    pagination.value.total = res.data.total || 0
  } catch (error) {
    ElMessage.error('加载工艺路线列表失败')
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
  loadRoutings()
}

const handlePageSizeChange = () => {
  loadRoutings()
}

const handlePageCurrentChange = () => {
  loadRoutings()
}

const showCreateDialog = () => {
  isEdit.value = false
  formData.value = { routingNumber: '', name: '', productId: '' }
  dialogVisible.value = true
}

const handleView = async (row: Routing) => {
  try {
    const res = await routingApi.getRouting(row.id)
    currentRouting.value = res.data
    detailDialogVisible.value = true
  } catch (error) {
    ElMessage.error('加载工艺路线详情失败')
  }
}

const handleEdit = (row: Routing) => {
  isEdit.value = true
  formData.value = {
    routingNumber: row.routingNumber,
    name: row.name,
    productId: row.productId
  }
  dialogVisible.value = true
}

const handleDelete = async (row: Routing) => {
  try {
    await ElMessageBox.confirm('确定要删除该工艺路线吗？', '提示', { type: 'warning' })
    await routingApi.deleteRouting(row.id)
    ElMessage.success('删除成功')
    loadRoutings()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

const handleSubmit = async () => {
  await formRef.value.validate()
  try {
    if (isEdit.value) {
      await routingApi.updateRouting({
        routingId: formData.value.productId, // 这里需要从currentRouting获取ID
        routingNumber: formData.value.routingNumber,
        name: formData.value.name
      })
      ElMessage.success('更新成功')
    } else {
      await routingApi.createRouting(formData.value as CreateRoutingRequest)
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    loadRoutings()
  } catch (error) {
    ElMessage.error(isEdit.value ? '更新失败' : '创建失败')
  }
}

const showAddOperationDialog = () => {
  operationForm.value = { sequence: 1, operationName: '', workCenterId: '', standardTime: 0 }
  operationDialogVisible.value = true
}

const handleAddOperation = async () => {
  await operationFormRef.value.validate()
  if (!currentRouting.value) return
  
  try {
    await routingApi.addOperation({
      routingId: currentRouting.value.id,
      ...operationForm.value
    })
    ElMessage.success('添加工序成功')
    operationDialogVisible.value = false
    await handleView({ id: currentRouting.value.id } as Routing)
  } catch (error) {
    ElMessage.error('添加工序失败')
  }
}

const handleDeleteOperation = async (operation: any) => {
  if (!currentRouting.value) return
  
  try {
    await ElMessageBox.confirm('确定要删除该工序吗？', '提示', { type: 'warning' })
    await routingApi.removeOperation(currentRouting.value.id, operation.sequence)
    ElMessage.success('删除工序成功')
    await handleView({ id: currentRouting.value.id } as Routing)
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('删除工序失败')
    }
  }
}

onMounted(() => {
  loadRoutings()
  loadProducts()
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

