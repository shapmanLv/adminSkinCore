import { getInfo } from '@/api/user'
import { logout } from '@/api/account'
// import { getToken, setToken, removeToken } from '@/utils/auth'
// import { resetRouter } from '@/router'

const getDefaultState = () => {
  return {
    name: '',
    account: '' // 登录名
  }
}

const state = getDefaultState()

const mutations = {
  RESET_STATE: (state) => {
    Object.assign(state, getDefaultState())
  },
  SET_TOKEN: (state, token) => {
    state.token = token
  },
  SET_NAME: (state, name) => {
    state.name = name
  },
  SET_AVATAR: (state, avatar) => {
    state.avatar = avatar
  },
  SET_ACCOUNT: (state, account) => {
    state.account = account
  }
}

const actions = {
  // get user info
  getInfo({ commit, state }) {
    return new Promise((resolve, reject) => {
      console.log('准备请求获取用户信息')
      getInfo().then(response => {
        const { data } = response
        console.log(data)
        if (!data) {
          return reject('Verification failed, please Login again.')
        }

        const { name, account } = data

        commit('SET_NAME', name)
        commit('SET_ACCOUNT', account)
        resolve(data)
      }).catch(error => {
        reject(error)
      })
    })
  },

  // user logout
  logout({ commit, state }) {
    return new Promise((resolve, reject) => {
      logout().then(() => {
        commit('RESET_STATE')
        resolve()
      }).catch(error => {
        reject(error)
      })
    })
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}

