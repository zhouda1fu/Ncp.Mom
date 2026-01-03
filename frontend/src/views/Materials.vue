<template>
  <div class="management-page">
    <div class="page-header">
      <div class="header-content">
        <div class="title-section">
          <h1>物料管理</h1>
          <p class="subtitle">管理系统中的所有物料信息</p>
        </div>
        <el-button 
          type="primary" 
          size="large"
          class="create-btn"
          @click="showCreateDialog"
          icon="Plus"
        >
          新建物料
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
                placeholder="请输入物料编码或名称"
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
        <el-table :data="materials" v-loading="loading" stripe>
          <el-table-column prop="materialCode" label="物料编码" min-width="150" />
          <el-table-column prop="materialName" label="物料名称" min-width="200" />
          <el-table-column prop="specification" label="规格" min-width="150" />
          <el-table-column prop="unit" label="单位" width="100" align="center" />
          <el-table-column label="操作" width="200" fixed="right" align="center">
            <template #default="{ row }">
              <el-button size="small" type="primary" @click="handleEdit(row)" :icon="Edit">编辑</el-button>
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
      :title="isEdit ? '编辑物料' : '新建物料'"
      width="500px"
    >
      <el-form :model="formData" :rules="formRules" ref="formRef" label-width="120px">
        <el-form-item label="物料编码" prop="materialCode">
          <el-input v-model="formData.materialCode" placeholder="请输入物料编码" />
        </el-form-item>
        <el-form-item label="物料名称" prop="materialName">
          <el-input v-model="formData.materialName" placeholder="请输入物料名称" />
        </el-form-item>
        <el-form-item label="规格">
          <el-input v-model="formData.specification" placeholder="请输入规格（可选）" />
        </el-form-item>
        <el-form-item label="单位" prop="unit">
          <el-input v-model="formData.unit" placeholder="请输入单位" />
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
import { materialApi, type Material, type CreateMaterialRequest, type UpdateMaterialRequest } from '@/api/material'

const loading = ref(false)
const materials = ref<Material[]>([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref()
const currentMaterial = ref<Material | null>(null)

const searchParams = ref({
  keyword: ''
})

const pagination = ref({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

const formData = ref({
  materialCode: '',
  materialName: '',
  specification: '',
  unit: '个'
})

const formRules = {
  materialCode: [{ required: true, message: '请输入物料编码', trigger: 'blur' }],
  materialName: [{ required: true, message: '请输入物料名称', trigger: 'blur' }],
  unit: [{ required: true, message: '请输入单位', trigger: 'blur' }]
}

const loadMaterials = async () => {
  loading.value = true
  try {
    const res = await materialApi.getMaterials({
      ...searchParams.value,
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize
    })
    materials.value = res.data.items || []
    pagination.value.total = res.data.total || 0
  } catch (error) {
    ElMessage.error('加载物料列表失败')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  pagination.value.pageIndex = 1
  loadMaterials()
}

const handlePageSizeChange = () => {
  loadMaterials()
}

const handlePageCurrentChange = () => {
  loadMaterials()
}

const showCreateDialog = () => {
  isEdit.value = false
  formData.value = {
    materialCode: '',
    materialName: '',
    specification: '',
    unit: '个'
  }
  dialogVisible.value = true
}

const handleEdit = (row: Material) => {
  isEdit.value = true
  currentMaterial.value = row
  formData.value = {
    materialCode: row.materialCode,
    materialName: row.materialName,
    specification: row.specification || '',
    unit: row.unit
  }
  dialogVisible.value = true
}

const handleDelete = async (row: Material) => {
  try {
    await ElMessageBox.confirm('确定要删除该物料吗？', '提示', {
      type: 'warning'
    })
    await materialApi.deleteMaterial(row.id)
    ElMessage.success('删除成功')
    loadMaterials()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

const handleSubmit = async () => {
  await formRef.value.validate()
  try {
    if (isEdit.value && currentMaterial.value) {
      await materialApi.updateMaterial({
        id: currentMaterial.value.id,
        ...formData.value
      })
      ElMessage.success('更新成功')
    } else {
      await materialApi.createMaterial(formData.value as CreateMaterialRequest)
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    loadMaterials()
  } catch (error) {
    ElMessage.error(isEdit.value ? '更新失败' : '创建失败')
  }
}

onMounted(() => {
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

