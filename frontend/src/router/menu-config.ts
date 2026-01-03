// 统一的菜单路由配置
export interface MenuRouteConfig {
  path: string
  name: string
  icon: string
  displayName: string
  component: () => Promise<any>
  requiredPermissions: string[]
  children?: MenuRouteConfig[]
  meta?: {
    requiresAuth?: boolean
    hideInMenu?: boolean
    [key: string]: any
  }
}

// 统一配置：一次配置，多处使用
export const menuRouteConfigs: MenuRouteConfig[] = [
  {
    path: '',
    name: 'Users',
    icon: 'User',
    displayName: '用户管理',
    component: () => import('@/views/Users.vue'),
    requiredPermissions: ['UserView', 'UserManagement']
  },
  {
    path: 'roles',
    name: 'Roles',
    icon: 'Setting',
    displayName: '角色管理',
    component: () => import('@/views/Roles.vue'),
    requiredPermissions: ['RoleView', 'RoleManagement']
  },
  {
    path: 'organization-units',
    name: 'OrganizationUnits',
    icon: 'OfficeBuilding',
    displayName: '组织架构',
    component: () => import('@/views/OrganizationUnits.vue'),
    requiredPermissions: ['OrganizationUnitView', 'OrganizationUnitManagement']
  },
  {
    path: 'products',
    name: 'Products',
    icon: 'Box',
    displayName: '产品管理',
    component: () => import('@/views/Products.vue'),
    requiredPermissions: []
  },
  {
    path: 'work-centers',
    name: 'WorkCenters',
    icon: 'OfficeBuilding',
    displayName: '工作中心管理',
    component: () => import('@/views/WorkCenters.vue'),
    requiredPermissions: []
  },
  {
    path: 'production-plans',
    name: 'ProductionPlans',
    icon: 'Document',
    displayName: '生产计划管理',
    component: () => import('@/views/ProductionPlans.vue'),
    requiredPermissions: []
  },
  {
    path: 'work-orders',
    name: 'WorkOrders',
    icon: 'Tickets',
    displayName: '工单管理',
    component: () => import('@/views/WorkOrders.vue'),
    requiredPermissions: []
  },
  {
    path: 'quality-inspections',
    name: 'QualityInspections',
    icon: 'DocumentChecked',
    displayName: '质量管理',
    component: () => import('@/views/QualityInspections.vue'),
    requiredPermissions: []
  },
  {
    path: 'equipments',
    name: 'Equipments',
    icon: 'Setting',
    displayName: '设备管理',
    component: () => import('@/views/Equipments.vue'),
    requiredPermissions: []
  },
  {
    path: 'boms',
    name: 'Boms',
    icon: 'List',
    displayName: 'BOM管理',
    component: () => import('@/views/Boms.vue'),
    requiredPermissions: []
  },
  {
    path: 'materials',
    name: 'Materials',
    icon: 'Box',
    displayName: '物料管理',
    component: () => import('@/views/Materials.vue'),
    requiredPermissions: []
  },
  {
    path: 'profile',
    name: 'Profile',
    icon: 'User',
    displayName: '个人资料',
    component: () => import('@/views/Profile.vue'),
    requiredPermissions: [],
    meta: { hideInMenu: true }
  }
]

// 将菜单配置转换为路由配置的辅助函数
export const convertToRouteConfig = (config: MenuRouteConfig) => ({
  path: config.path,
  name: config.name,
  component: config.component,
  meta: {
    permissions: config.requiredPermissions,
    requiresAuth: true,
    ...config.meta
  }
})

// 将菜单配置转换为菜单权限配置的辅助函数
export const convertToMenuPermission = (config: MenuRouteConfig) => ({
  path: config.path === '' ? '/' : `/${config.path}`,
  name: config.displayName,
  icon: config.icon,
  requiredPermissions: config.requiredPermissions
}) 