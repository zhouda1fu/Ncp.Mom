<template>
  <div class="management-page">
    <div class="page-header">
      <div class="header-content">
        <div class="title-section">
          <h1>BOM管理</h1>
          <p class="subtitle">管理系统中的所有物料清单信息</p>
        </div>
        <el-button 
          type="primary" 
          size="large"
          class="create-btn"
          @click="showCreateDialog"
          icon="Plus"
        >
          新建BOM
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
                placeholder="请输入BOM编号"
                clearable
                class="search-input"
                :prefix-icon="Search"
                @keyup.enter="handleSearch"
              />
            </el-form-item>
            <el-form-item label="产品">
              <el-select v-model="searchParams.productId" placeholder="请选择产品" clearable filterable>
                <el-option
                  v-for="product in products"
                  :key="product.id"
                  :label="`${product.productCode} - ${product.productName}`"
                  :value="product.id"
                />
              </el-select>
            </el-form-item>
            <el-form-item label="状态">
              <el-select v-model="searchParams.isActive" placeholder="请选择状态" clearable>
                <el-option label="激活" :value="true" />
                <el-option label="停用" :value="false" />
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
        <el-table :data="boms" v-loading="loading" stripe>
          <el-table-column prop="bomNumber" label="BOM编号" min-width="150" />
          <el-table-column prop="productId" label="产品ID" min-width="150" />
          <el-table-column prop="version" label="版本" width="100" align="center" />
          <el-table-column prop="items" label="物料数量" width="100" align="center">
            <template #default="{ row }">
              {{ row.items?.length || 0 }}
            </template>
          </el-table-column>
          <el-table-column label="状态" width="100" align="center">
            <template #default="{ row }">
              <el-tag :type="row.isActive ? 'success' : 'info'">
                {{ row.isActive ? '激活' : '停用' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="updateTime" label="更新时间" width="160" />
          <el-table-column label="操作" width="250" fixed="right" align="center">
            <template #default="{ row }">
              <el-button size="small" type="info" @click="handleView(row)" :icon="View">查看</el-button>
              <el-button 
                v-if="row.isActive" 
                size="small" 
                type="warning" 
                @click="handleDeactivate(row)" 
                :icon="Close"
              >
                停用
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
      title="新建BOM"
      width="500px"
    >
      <el-form :model="formData" :rules="formRules" ref="formRef" label-width="120px">
        <el-form-item label="BOM编号" prop="bomNumber">
          <el-input v-model="formData.bomNumber" placeholder="请输入BOM编号" />
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
        <el-form-item label="版本" prop="version">
          <el-input-number v-model="formData.version" :min="1" style="width: 100%" />
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
      title="BOM详情"
      width="800px"
    >
      <div v-if="currentBom">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="BOM编号">{{ currentBom.bomNumber }}</el-descriptions-item>
          <el-descriptions-item label="产品ID">{{ currentBom.productId }}</el-descriptions-item>
          <el-descriptions-item label="版本">{{ currentBom.version }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="currentBom.isActive ? 'success' : 'info'">
              {{ currentBom.isActive ? '激活' : '停用' }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="更新时间">{{ currentBom.updateTime }}</el-descriptions-item>
        </el-descriptions>
        
        <div style="margin-top: 20px;">
          <div style="display: flex; justify-content: space-between; margin-bottom: 10px;">
            <h3>物料清单</h3>
            <el-button type="primary" size="small" @click="showAddItemDialog" icon="Plus">添加物料</el-button>
          </div>
          <el-table :data="currentBom.items" stripe>
            <el-table-column prop="materialId" label="物料ID" />
            <el-table-column prop="quantity" label="数量" width="120" align="center" />
            <el-table-column prop="unit" label="单位" width="100" align="center" />
            <el-table-column label="操作" width="100" align="center">
              <template #default="{ row }">
                <el-button size="small" type="danger" @click="handleDeleteItem(row)" :icon="Delete">删除</el-button>
              </template>
            </el-table-column>
          </el-table>
        </div>
      </div>
    </el-dialog>

    <!-- 添加物料对话框 -->
    <el-dialog
      v-model="itemDialogVisible"
      title="添加物料"
      width="500px"
    >
      <el-form :model="itemForm" :rules="itemRules" ref="itemFormRef" label-width="120px">
        <el-form-item label="物料" prop="materialId">
          <el-select v-model="itemForm.materialId" placeholder="请选择物料" filterable style="width: 100%">
            <el-option
              v-for="material in materials"
              :key="material.id"
              :label="`${material.materialCode} - ${material.materialName}`"
              :value="material.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="数量" prop="quantity">
          <el-input-number v-model="itemForm.quantity" :min="0.01" :precision="2" style="width: 100%" />
        </el-form-item>
        <el-form-item label="单位" prop="unit">
          <el-input v-model="itemForm.unit" placeholder="请输入单位" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="itemDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleAddItem">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, View, Close, Delete } from '@element-plus/icons-vue'
import { bomApi, type Bom, type CreateBomRequest, type AddBomItemRequest, type BomItem } from '@/api/bom'
import { productApi, type Product } from '@/api/product'
import { materialApi, type Material } from '@/api/material'

const loading = ref(false)
const boms = ref<Bom[]>([])
const products = ref<Product[]>([])
const materials = ref<Material[]>([])
const dialogVisible = ref(false)
const detailDialogVisible = ref(false)
const itemDialogVisible = ref(false)
const formRef = ref()
const itemFormRef = ref()
const currentBom = ref<Bom | null>(null)

const searchParams = ref({
  keyword: '',
  productId: undefined as string | undefined,
  isActive: undefined as boolean | undefined
})

const pagination = ref({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

const formData = ref({
  bomNumber: '',
  productId: '',
  version: 1
})

const itemForm = ref({
  materialId: '',
  quantity: 0,
  unit: '个'
})

const formRules = {
  bomNumber: [{ required: true, message: '请输入BOM编号', trigger: 'blur' }],
  productId: [{ required: true, message: '请选择产品', trigger: 'change' }],
  version: [{ required: true, message: '请输入版本', trigger: 'blur' }]
}

const itemRules = {
  materialId: [{ required: true, message: '请选择物料', trigger: 'change' }],
  quantity: [{ required: true, message: '请输入数量', trigger: 'blur' }],
  unit: [{ required: true, message: '请输入单位', trigger: 'blur' }]
}

const loadBoms = async () => {
  loading.value = true
  try {
    const res = await bomApi.getBoms({
      ...searchParams.value,
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize
    })
    boms.value = res.data.items || []
    pagination.value.total = res.data.total || 0
  } catch (error) {
    ElMessage.error('加载BOM列表失败')
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

const loadMaterials = async () => {
  try {
    const res = await materialApi.getMaterials({ pageIndex: 1, pageSize: 1000 })
    materials.value = res.data.items || []
  } catch (error) {
    ElMessage.error('加载物料列表失败')
  }
}

const handleSearch = () => {
  pagination.value.pageIndex = 1
  loadBoms()
}

const handlePageSizeChange = () => {
  loadBoms()
}

const handlePageCurrentChange = () => {
  loadBoms()
}

const showCreateDialog = () => {
  formData.value = {
    bomNumber: '',
    productId: '',
    version: 1
  }
  dialogVisible.value = true
}

const handleView = async (row: Bom) => {
  try {
    const res = await bomApi.getBom(row.id)
    currentBom.value = res.data
    detailDialogVisible.value = true
  } catch (error) {
    ElMessage.error('加载BOM详情失败')
  }
}

const handleDeactivate = async (row: Bom) => {
  try {
    await ElMessageBox.confirm('确定要停用该BOM吗？', '提示', {
      type: 'warning'
    })
    await bomApi.deactivateBom(row.id)
    ElMessage.success('停用成功')
    loadBoms()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('停用失败')
    }
  }
}

const showAddItemDialog = () => {
  itemForm.value = {
    materialId: '',
    quantity: 0,
    unit: '个'
  }
  itemDialogVisible.value = true
}

const handleAddItem = async () => {
  if (!currentBom.value) return
  
  await itemFormRef.value.validate()
  try {
    await bomApi.addBomItem({
      id: currentBom.value.id,
      materialId: itemForm.value.materialId,
      quantity: itemForm.value.quantity,
      unit: itemForm.value.unit
    })
    ElMessage.success('添加物料成功')
    itemDialogVisible.value = false
    await handleView({ id: currentBom.value.id } as Bom)
  } catch (error) {
    ElMessage.error('添加物料失败')
  }
}

const handleDeleteItem = async (item: BomItem) => {
  if (!currentBom.value) return
  
  try {
    await ElMessageBox.confirm('确定要删除该物料吗？', '提示', {
      type: 'warning'
    })
    await bomApi.removeBomItem(currentBom.value.id, item.id)
    ElMessage.success('删除物料成功')
    await handleView({ id: currentBom.value.id } as Bom)
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('删除物料失败')
    }
  }
}

const handleSubmit = async () => {
  await formRef.value.validate()
  try {
    await bomApi.createBom(formData.value as CreateBomRequest)
    ElMessage.success('创建成功')
    dialogVisible.value = false
    loadBoms()
  } catch (error) {
    ElMessage.error('创建失败')
  }
}

onMounted(() => {
  loadBoms()
  loadProducts()
  loadMaterials()
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

