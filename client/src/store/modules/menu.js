import { GetUserMenuTree } from '@/api/menu'
//  import router from '@/router'
import { filterRoutes } from '@/router'

const createMenuRouter = data => {
  const array = []
  for (let index = 0; index < data.length; index++) {
    const element = data[index]
    const temp = {
      path: element.routerPath,
      name: element.id,
      hidden: element.hidden,
      component: () => import('' + element.componentRelativePath),
      meta: {
        path: element.routerPath,
        title: element.name,
        icon: element.icon,
        routerName: element.routerName
      },
      children: []
    }
    if (element.children && element.children.length > 0) {
      temp.children = createMenuRouter(element.children)
    }

    array.push(temp)
  }
  return array
}

const state = {
  menuTree: []
}

const mutations = {
  SET_MENU: (state, menuTree) => {
    state.menuTree = menuTree
  }
}

const actions = {
  // user login
  buildUserMenuTree({ commit }) {
    console.log('准备获取菜单信息')
    GetUserMenuTree().then(res => {
      if (res.code === 200) {
        localStorage.setItem('menuTreeData', JSON.stringify(res.data)) // 存储到客户端本地
        this.refreshUserMenuTree(res.data)
      }
    })
  },
  refreshUserMenuTree({ commit }, menuTreeData) {
    let array = []
    array = createMenuRouter(menuTreeData)
    commit('SET_MENU', array)

    filterRoutes(menuTreeData) // 做动态路由
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}

