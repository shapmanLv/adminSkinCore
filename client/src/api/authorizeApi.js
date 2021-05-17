import request from '@/utils/request'

export function GetAuthorizeApiList(data) {
  return request({
    url: `/api/AuthorizeApi/GetAuthorizeApiPageData`,
    method: 'post',
    data
  })
}

export function GetAllAuthorizeApi() {
  return request({
    url: `/api/AuthorizeApi/GetAllAuthorizeApi`,
    method: 'get'
  })
}

export function AddAuthorizeApi(data) {
  return request({
    url: `/api/AuthorizeApi/AddAuthorizeApi`,
    method: 'post',
    data
  })
}

export function EditAuthorizeApi(data) {
  return request({
    url: `/api/AuthorizeApi/EditAuthorizeApi`,
    method: 'put',
    data
  })
}

export function RemoveAuthorizeApi(data) {
  return request({
    url: `/api/AuthorizeApi/RemoveAuthorizeApi/${data}`,
    method: 'delete'
  })
}

export function BatchRemoveAuthorizeApi(data) {
  return request({
    url: `/api/AuthorizeApi/BatchRemoveAuthorizeApi`,
    method: 'delete',
    data
  })
}

export function BuildAuthorizeApis() {
  return request({
    url: '/api/AuthorizeApi/BuildAuthorizeApis',
    method: 'post'
  })
}
