<template>
  <div class="management-page">
    <div class="page-header">
      <div class="header-content">
        <div class="title-section">
          <h1>产品管理</h1>
          <p class="subtitle">管理系统中的所有产品信息</p>
        </div>
        <el-button 
          type="primary" 
          size="large"
          class="create-btn"
          @click="showCreateDialog"
          icon="Plus"
        >
          新建产品
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
                placeholder="请输入产品编码或名称"
                clearable
                class="search-input"
                :prefix-icon="Search"
                @keyup.enter="handleSearch"
              />
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
        <el-table :data="products" v-loading="loading" stripe>
          <el-table-column prop="productCode" label="产品编码" min-width="150" />
          <el-table-column prop="productName" label="产品名称" min-width="200" />
          <el-table-column label="操作" width="150" fixed="right" align="center">
            <template #default="{ row }">
              <el-button size="small" type="primary" @click="handleEdit(row)" :icon="Edit" />
              <el-button size="small" type="danger" @click="handleDelete(row)" :icon="Delete" />
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
      :title="isEdit ? '编辑产品' : '新建产品'"
      width="500px"
    >
      <el-form :model="formData" :rules="formRules" ref="formRef" label-width="100px">
        <el-form-item label="产品编码" prop="productCode">
          <el-input v-model="formData.productCode" placeholder="请输入产品编码" />
        </el-form-item>
        <el-form-item label="产品名称" prop="productName">
          <el-input v-model="formData.productName" placeholder="请输入产品名称" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Edit, Delete } from '@element-plus/icons-vue'
import { productApi, type Product, type CreateProductRequest, type UpdateProductRequest } from '@/api/product'

const loading = ref(false)
const products = ref<Product[]>([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref()
const currentProduct = ref<Product | null>(null)

const searchParams = ref({
  keyword: ''
})

const pagination = ref({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

const formData = ref({
  productCode: '',
  productName: ''
})

const formRules = {
  productCode: [{ required: true, message: '请输入产品编码', trigger: 'blur' }],
  productName: [{ required: true, message: '请输入产品名称', trigger: 'blur' }]
}

const loadProducts = async () => {
  loading.value = true
  try {
    const res = await productApi.getProducts({
      ...searchParams.value,
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize
    })
    products.value = res.data.items || []
    pagination.value.total = res.data.total || 0
  } catch (error) {
    ElMessage.error('加载产品列表失败')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  pagination.value.pageIndex = 1
  loadProducts()
}

const handlePageSizeChange = () => {
  loadProducts()
}

const handlePageCurrentChange = () => {
  loadProducts()
}

const showCreateDialog = () => {
  isEdit.value = false
  formData.value = { productCode: '', productName: '' }
  dialogVisible.value = true
}

const handleEdit = (row: Product) => {
  isEdit.value = true
  currentProduct.value = row
  formData.value = {
    productCode: row.productCode,
    productName: row.productName
  }
  dialogVisible.value = true
}

const handleDelete = async (row: Product) => {
  try {
    await ElMessageBox.confirm('确定要删除该产品吗？', '提示', {
      type: 'warning'
    })
    await productApi.deleteProduct(row.id)
    ElMessage.success('删除成功')
    loadProducts()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

const handleSubmit = async () => {
  await formRef.value.validate()
  try {
    if (isEdit.value && currentProduct.value) {
      await productApi.updateProduct({
        productId: currentProduct.value.id,
        ...formData.value
      })
      ElMessage.success('更新成功')
    } else {
      await productApi.createProduct(formData.value)
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    loadProducts()
  } catch (error) {
    ElMessage.error(isEdit.value ? '更新失败' : '创建失败')
  }
}

onMounted(() => {
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

