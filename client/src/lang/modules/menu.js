export const menu_en = {
  menuPage: {
    table: {
      number: 'index',
      menuName: 'name',
      routerPath: 'router path',
      routerName: 'router key',
      desc: 'description',
      sort: 'order by',
      hidden: 'hidden',
      hiddenUiTag: 'yes', // hidden值为true时，ui中tag组件显示的内容
      notHiddenUiTag: 'no', // hidden值为false时，ui中tag组件显示的内容
      action: 'action'
    },
    form: {
      icon: 'icon',
      parentMenu: 'parent node',
      sortTooltipContent: 'The larger the value is, the larger the row is, the smallest is 0, and the largest is 99',
      rule: {
        menuName: 'Please enter the menu name',
        parentIdStr: 'The parent node should not be equal to itself'
      }
    },
    placeholder: {
      parentMenu: 'If it is a top-level node, leave it blank'
    },
    editError: {
      10001: 'This menu record does not exist and cannot be modified'
    },
    addError: {
      10001: 'A menu record with the same name and the same parent node already exists'
    }
  }
}

export const menu_zh = {
  menuPage: {
    table: {
      number: '序号',
      menuName: '菜单',
      routerPath: '路由地址',
      routerName: '路由标识',
      desc: '描述',
      sort: '排序值',
      hidden: '是否隐藏',
      hiddenUiTag: '隐藏', // hidden值为true时，ui中tag组件显示的内容
      notHiddenUiTag: '显示', // hidden值为false时，ui中tag组件显示的内容
      action: '操作'
    },
    form: {
      icon: '图标',
      parentMenu: '父级菜单',
      sortTooltipContent: '值越大排的越前，最小为0，最大为99',
      rule: {
        menuName: '请输入菜单名称',
        parentIdStr: '父级节点不应该等于自己'
      }
    },
    tooltip: {
      name: ''
    },
    placeholder: {
      icon: '请选择图标',
      parentMenu: '如果是顶级节点，请留空'
    },
    editError: {
      10001: '此菜单记录不存在，无法修改'
    },
    addError: {
      10001: '已经存在一个同级且同名的菜单节点，请尝试换个名字，或者更换一个层级'
    }
  }
}
