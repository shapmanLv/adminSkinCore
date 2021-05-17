import axios from 'axios'
import { Message } from 'element-ui'
import store from '@/store'
import router from '@/router'
// import router from 'vue-router'

// create an axios instance
const service = axios.create({
  baseURL: process.env.VUE_APP_BASE_API, // url = base url + request url
  withCredentials: true, // send cookies when cross-domain requests
  timeout: 10000 // request timeout
})

// request interceptor
service.interceptors.request.use(
  config => {
    // do something before request is sent

    // if (store.getters.token) {
    //   // let each request carry token
    //   // ['X-Token'] is a custom headers key
    //   // please modify it according to the actual situation
    //   config.headers['X-Token'] = getToken()
    // }

    return config
  },
  error => {
    // do something with request error
    console.log(error) // for debug
    return Promise.reject(error)
  }
)

// response interceptor
service.interceptors.response.use(
  /**
   * If you want to get http information such as headers or status
   * Please return  response => response
  */

  /**
   * Determine the request status by custom code
   * Here is just an example
   * You can also judge the status by HTTP Status Code
   */
  response => {
    const res = response.data
    console.log(res)
    return res
  },
  error => {
    if (parseInt(error.response.status) === 403) {
      if (router.currentRoute.path !== '/login') {
        Message({
          message: '登录信息过期，请重新登录。',
          type: 'warning',
          duration: 5 * 1000
        })
        store.dispatch('user/logout')
        store.dispatch('app/updateRouterSettingState', false)
        router.push(`/login?redirect=/`)
      }
    } else if (parseInt(error.response.status) === 401) {
      router.replace({ path: '/error/401' })
    } else {
      console.log('err' + error) // for debug
      Message({
        message: error.message,
        type: 'error',
        duration: 5 * 1000
      })
    }
    return Promise.reject(error)
  }
)

export default service
