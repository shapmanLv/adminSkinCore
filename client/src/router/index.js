import Vue from 'vue'
import Router from 'vue-router'
// import store from '../store'

Vue.use(Router)

/* Layout */
import Layout from '@/layout'
// import { title } from '@/settings'

// 固定的路由，公共页面
const constantRoutes = [
  {
    path: '/login',
    component: () => import('@/views/login/index')
  },
  {
    name: '404page',
    path: '/error',
    component: Layout,
    children: [{
      path: '/error/404',
      name: '404',
      component: () => import('@/views/404'),
      meta: {
        title: '404'
      }
    }]
  },
  {
    path: '*',
    redirect: '/error/404'
  },
  {
    name: '401page',
    path: '/error',
    component: Layout,
    children: [{
      path: '/error/401',
      name: '401',
      component: () => import('@/views/401'),
      meta: {
        title: '401'
      }
    }]
  },
  {
    path: '/',
    component: Layout,
    children: [{
      path: '/',
      name: 'home',
      component: () => import('@/views/home/index'),
      meta: {
        routerName: 'home',
        redirect: true,
        noCache: true,
        title: '首页'
      }
    }]
  }
]

// 需要权限才能访问的页面，路由为后端控制
const appRoutes = [
  {
    name: 'User',
    component: () => import('../views/user/index.vue'),
    noCache: false
  },
  {
    name: 'Menu',
    component: () => import('../views/menu/index.vue'),
    noCache: false
  },
  {
    name: 'Role',
    component: () => import('../views/role/index.vue'),
    noCache: false
  },
  {
    name: 'AuthorizeApi',
    component: () => import('../views/authorizeApi/index.vue'),
    noCache: false
  }
]

/**
 * 对需要权限的路由进行筛选，添加到用户的路由中
 * @param {*} data 用户菜单
 */
export const filterRoutes = data => {
  console.log('准备开始动态添加路由')
  var array = []
  eachChildren(data, array)
  console.log('筛选出来的数据为：')
  console.log(array)
  resetRouter()
  router.addRoutes(array)
}

/**
 * 遍历菜单树
 * @param {*} data 菜单树子树
 */
const eachChildren = (data, array) => {
  for (let index = 0; index < data.length; index++) {
    const element = data[index]
    if (element.children && element.children.length > 0) { // 分组菜单
      var groupMenu = {
        path: element.routerPath,
        name: element.routerName,
        component: Layout,
        meta: {
          routerName: element.routerName,
          redirect: false,
          title: element.name
        },
        children: []
      }

      eachChildren(element.children, groupMenu.children)
      array.push(groupMenu)
      continue
    }

    for (let i = 0; i < appRoutes.length; i++) {
      const el = appRoutes[i]

      if (el.name === element.routerName) {
        var temp = {
          path: element.routerPath,
          name: el.name, // el.component
          component: el.component, // el.component, // () => require('@/views/' + el.name + '/index.vue'),
          meta: {
            routerName: element.routerName,
            redirect: true,
            noCache: el.noCache,
            title: element.name
          }
        }

        if (element.children && element.children.length > 0) {
          eachChildren(element.children, temp.children)
        }

        if (element.parentId === 0) { // 他是有页面跳转逻辑的一级菜单
          temp = {
            path: element.routerPath,
            component: Layout,
            children: [temp]
          }
        }

        array.push(temp)
      }
    }
  }
}

/** 初始化路由 */
const createRouter = () => new Router({
  // mode: 'history', // require service support
  scrollBehavior: () => ({ y: 0 }),
  routes: constantRoutes
})

/** 重置路由，动态设置的路由通过此法，实现重置 */
export function resetRouter() {
  const newRouter = createRouter()
  router.matcher = newRouter.matcher // reset router
}

const router = createRouter() // 路由对象

export default router
