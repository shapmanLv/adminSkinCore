import request from '@/utils/request'

export function getInfo() {
  return request({
    url: '/api/User/GetUserInfo',
    method: 'get'
  })
}

export function GetUserList(data) {
  return request({
    url: `/api/User/GetUserPageData`,
    method: 'post',
    data
  })
}

export function GetAllUser() {
  return request({
    url: `/api/User/GetAllUser`,
    method: 'get'
  })
}

export function AddUser(data) {
  return request({
    url: `/api/User/AddUser`,
    method: 'post',
    data
  })
}

export function EditUser(data) {
  return request({
    url: `/api/User/EditUser`,
    method: 'put',
    data
  })
}

export function RemoveUser(data) {
  return request({
    url: `/api/User/RemoveUser/${data}`,
    method: 'delete'
  })
}

export function BatchRemoveUser(data) {
  return request({
    url: `/api/User/BatchRemoveUser`,
    method: 'delete',
    data
  })
}
