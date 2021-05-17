import request from '@/utils/request'

export function GetMenuTree() {
  return request({
    url: `/api/Menu/GetMenuTree`,
    method: 'get'
  })
}

export function GetUserMenuTree() {
  return request({
    url: '/api/Menu/GetUserMenuTree',
    method: 'get'
  })
}

export function AddMenu(data) {
  return request({
    url: `/api/Menu/AddMenu`,
    method: 'post',
    data
  })
}

export function EditMenu(data) {
  return request({
    url: `/api/Menu/EditMenu`,
    method: 'put',
    data
  })
}

export function RemoveMenu(data) {
  return request({
    url: `/api/Menu/RemoveMenu/${data}`,
    method: 'delete'
  })
}
