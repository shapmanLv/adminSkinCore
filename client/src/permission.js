import router from './router'
import store from './store'
// import { Message } from 'element-ui'
import NProgress from 'nprogress' // progress bar
import 'nprogress/nprogress.css' // progress bar style
import getPageTitle from '@/utils/get-page-title'
import { GetUserMenuTree } from '@/api/menu'

NProgress.configure({ showSpinner: false }) // NProgress Configuration

router.beforeEach(async(to, from, next) => {
  // start progress bar
  NProgress.start()

  // set page title
  document.title = getPageTitle(to.meta.title)

  if (store.getters.hasSettingRouter || to.path === '/login') { // 动态路由部分已经做过，直接跳转
    if (to.path !== '/error/404' && to.path !== '/error/401' && to.path !== '/login') {
      localStorage.setItem('accessHistory', to.path) // 存储用户的跳转记录
    }
    next()
  } else {
    console.log('开始请求后台菜单数据')
    await GetUserMenuTree().then(res => {
      store.dispatch('menu/refreshUserMenuTree', res.data)
      store.dispatch('app/updateRouterSettingState', true)
      console.log('准备就绪，继续执行跳转')
      console.log('跳转来自于：' + from.path)
      console.log('要去往：' + to.path)
      if (to.path === '/error/404') {
        next(localStorage.getItem('accessHistory') || '/')
      }
      next()
      store.dispatch('user/getInfo') // 菜单初始化成功之后，加载用户信息
    }).catch(res => {
      console.log(res)
      console.log('请求失败，继续执行跳转')
      // if (res.response.status === 403) {
      //   console.log('导航守卫加载菜单数据失败，发现返回的是403，准备改成跳转至登录页面')
      //   next('/login')
      // }
      next()
    })
  }
})

router.afterEach(() => {
  // finish progress bar
  NProgress.done()
})
