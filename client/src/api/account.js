import request from '@/utils/request'

export function login(data) {
  return request({
    url: '/api/Account/Login',
    method: 'post',
    data
  })
}

export function sendVerificationCode(username) {
  return request({
    url: '/api/Account/SendCaptcha/' + username,
    method: 'post'
  })
}

export function UpdatePassword(data) {
  return request({
    url: `/api/Account/UpdatePassword`,
    method: 'put',
    data
  })
}

export function logout() {
  return request({
    url: '/api/Account/Logout',
    method: 'post'
  })
}
