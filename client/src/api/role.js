import request from '@/utils/request'

export function GetRoleList(data) {
  return request({
    url: `/api/Role/GetRolePageData`,
    method: 'post',
    data
  })
}

export function GetAllRole() {
  return request({
    url: '/api/Role/GetAllRole',
    method: 'get'
  })
}

export function AddRole(data) {
  return request({
    url: `/api/Role/AddRole`,
    method: 'post',
    data
  })
}

export function EditRole(data) {
  return request({
    url: `/api/Role/EditRole`,
    method: 'put',
    data
  })
}

export function RemoveRole(data) {
  return request({
    url: `/api/Role/RemoveRole/${data}`,
    method: 'delete'
  })
}

export function BatchRemoveRole(data) {
  return request({
    url: `/api/Role/BatchRemoveRole`,
    method: 'delete',
    data
  })
}
