# AdminSkinCore
基于.net 5和 vue2 实现的后台项目模板，内部包含基本的权限控制功能及定时任务等扩展功能    

**前端代码参考自：[vue-element-admin](https://github.com/PanJiaChen/vue-element-admin)**

### 运行后效果图：
![图片示例](https://images.gitee.com/uploads/images/2021/0122/233103_ef40172e_5471744.png "图片示例")

## 页面具体内容  
- 列表展示需授权才可使用的全部接口
- 角色管理
- 前端页面菜单管理
- 用户管理

## 使用方式

当你有一个新的页面需要添加到侧边栏菜单时，首先你需要在vue 页面中确定一个不重复的**name**

```javascript
<script>
export default {
  name: 'Demo',
}
</script>
```

然后在 **/src/router/index.js** 文件中，添加路由

```javascript
  {
    name: 'Demo',
    component: () => import('../views/demo/index.vue'),
    noCache: false // 表示这个页面是否使用缓存，false表示开启
  }
```

接下来在网站运行后的**菜单管理**页面，进行菜单添加。其中的**路由标识**一项，需填写的是Vue页面组件的name值，如果该菜单是分组菜单，不跳转至实际页面，则随意取一个不重复的值填写即可。

