<template>
  <div class="app-container">
    <div
      class="filter-container"
      style="margin-bottom: 20px"
    >
      <el-button
        v-show="showAdd"
        class="filter-item"
        style="margin-left: 10px;"
        type="primary"
        icon="el-icon-plus"
        size="small"
        @click="handerShowCreateDialog()"
      >
        添加
      </el-button>
    </div>

    <el-table
      v-loading="listLoading"
      :data="list"
      highlight-current-row
      row-key="id"
      border
      lazy
      :header-cell-style="{background:'#F3F4F7',color:'#555'}"
      :tree-props="{children: 'children', hasChildren: 'hasChildren'}"
      size="small"
      style="width: 100%;"
    >
      <el-table-column
        type="index"
        label="序号"
        width="80"
      />
      <el-table-column
        label="菜单"
        width="200"
      >
        <template slot-scope="{ row }">
          <i :class="row.icon" />
          <span> {{ row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column
        prop="routerPath"
        label="路由地址"
      />
      <el-table-column
        prop="routerName"
        label="路由标识"
      />
      <el-table-column
        prop="desc"
        label="描述"
      />
      <el-table-column
        prop="sort"
        label="排序值"
        width
      />
      <el-table-column
        prop="hidden"
        label="是否隐藏"
        width="100"
      >
        <template slot-scope="scope">
          <el-tag
            :type="scope.row.hidden ? 'danger' : 'success'"
            disable-transitions
          >{{ scope.row.hidden ? '隐藏': '显示' }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column
        label="操作"
        align="center"
        width="100"
        fixed="right"
        class-name="small-padding fixed-width"
      >
        <template slot-scope="{row,$index}">
          <el-button
            type="primary"
            icon="el-icon-edit"
            size="small"
            circle
            @click="handleShowUpdateDialog(row.id)"
          />
          <el-button
            type="danger"
            icon="el-icon-delete"
            size="small"
            circle
            @click="handleDelete(row.id,$index)"
          />
        </template>
      </el-table-column>
    </el-table>

    <el-dialog
      :visible.sync="dialogVisible"
      :title="dialogTitle"
      :append-to-body="true"
    >
      <el-form
        ref="formData"
        label-width="120px"
        :model="formData"
        :rules="formRules"
      >
        <!-- v-show="isAddForm" -->
        <el-form-item
          label="菜单名"
          prop="name"
        >
          <el-input
            v-model="formData.name"
            maxlength="10"
            show-word-limit
          />
        </el-form-item>
        <el-form-item
          label="路由地址"
          prop="routerPath"
        >
          <el-input
            v-model="formData.routerPath"
            maxlength="50"
            show-word-limit
          />
        </el-form-item>
        <el-form-item
          prop="parentIdStr"
          label="父级"
          width
          sortable
        >
          <el-cascader
            v-model="parentIdArray"
            :options="list"
            style="width:100%"
            :props="{ checkStrictly: true, expandTrigger: 'hover', value: 'id', label: 'name' }"
            placeholder="如果是顶级菜单，该项请留空"
            clearable
          />
        </el-form-item>
        <!-- v-show="isAddForm" -->
        <el-form-item
          label="路由标识"
          prop="routerName"
        >
          <el-input
            v-model="formData.routerName"
            placeholder="这一项内容需要填写的是Vue组件中的name值，且需保证不与其他相重复"
            maxlength="20"
            show-word-limit
          />
        </el-form-item>
        <el-form-item
          label="描述"
          prop="desc"
        >
          <el-input
            v-model="formData.desc"
            type="textarea"
            maxlength="50"
            show-word-limit
          />
        </el-form-item>
        <el-form-item
          label="是否隐藏"
          prop="hidden"
        >
          <el-switch
            v-model="formData.hidden"
            active-color="#13ce66"
            inactive-color="#F3F3F3"
          />
        </el-form-item>
        <el-form-item
          label="排序值"
          prop="sort"
        >
          <el-tooltip
            class="item"
            effect="dark"
            content="值越大排的越前，最小为0，最大为99"
            placement="top-start"
          >
            <el-input-number
              v-model="formData.sort"
              :min="0"
              :max="99"
              size="small"
            />
          </el-tooltip>
        </el-form-item>
        <el-form-item
          label="图标"
          prop="icon"
        >
          <e-icon-picker
            v-model="formData.icon"
          />
        </el-form-item>
      </el-form>
      <div
        slot="footer"
        class="dialog-footer"
      >
        <el-button @click.native="closeFormDialog">取消</el-button>
        <el-button
          type="primary"
          :loading="formLoading"
          @click.native="submit('formData')"
        >确定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import elDragDialog from '@/directive/el-drag-dialog'
import { GetMenuTree, EditMenu, AddMenu, RemoveMenu } from '@/api/menu'

export default {
  name: 'AuthorizeApi',
  directives: { elDragDialog },
  data() {
    const menuNameValidate = (rule, value, callback) => {
      if (value) {
        callback()
      } else {
        callback(new Error('请输入菜单名'))
      }
    }
    return {
      showAdd: process.env.NODE_ENV === 'development',
      list: [],
      listLoading: true,
      dialogVisible: false,
      dialogTitle: '', // 编辑/新增
      isAddForm: false,
      formData: {
        id: 0,
        name: '',
        routerPath: '',
        routerName: '',
        desc: '',
        icon: '',
        hidden: false,
        parentId: '',
        parentIdStr: '',
        sort: 0
      },
      parentIdArray: [],
      formLoading: false,
      formRules: {
        name: [{ required: true, trigger: 'blur', validator: menuNameValidate }]
      }
    }
  },
  created() {
    this.getList()
  },
  methods: {
    /** 获取表格数据 */
    getList() {
      this.listLoading = true
      GetMenuTree().then(res => {
        if (res.code === 200) {
          this.list = res.data
        } else {
          this.$notify.error({
            title: '操作错误',
            message: '加载数据失败',
            duration: 2000
          })

          this.listLoading = false
        }

        setTimeout(() => {
          this.listLoading = false
        }, 300)
      }).catch(res => { this.listLoading = false })
    },
    /** 搜索按钮点下，筛选数据 */
    handleFilter() {
      this.listQuery.page = 1 // 搜索的时候，页码都重置为1
      this.getList()
    },
    /** 显示新增表单 */
    handerShowCreateDialog() {
      this.formData.id = 0
      this.formData.name = ''
      this.formData.routerPath = ''
      this.formData.routerName = ''
      this.formData.icon = ''
      this.formData.hidden = false
      this.formData.parentId = 0
      this.formData.sort = ''
      this.formData.parentIdStr = ''
      this.parentIdArray = []
      this.formData.desc = ''
      this.dialogTitle = '添加'
      this.isAddForm = true
      this.dialogVisible = true
    },
    /** 显示修改表单 */
    handleShowUpdateDialog(id) {
      const temp = this.findMenuNode(this.list, id)
      console.log('查询出来的数据：')
      console.log(temp)
      if (temp !== null) {
        this.formData.id = temp.id
        this.formData.name = temp.name
        this.formData.routerPath = temp.routerPath
        this.formData.routerName = temp.routerName
        this.formData.icon = temp.icon
        this.formData.hidden = temp.hidden
        this.formData.parentId = temp.parentId
        this.formData.sort = temp.sort
        this.formData.desc = temp.desc
        this.formData.parentIdStr = temp.parentIdStr
        if (temp.parentIdStr) {
          this.parentIdArray = temp.parentIdStr.split('/').map(u => parseInt(u))
        }

        this.dialogTitle = '修改'
        this.isAddForm = false
        this.dialogVisible = true
      } else {
        console.log('未找到记录')
      }
    },
    findMenuNode(dataSource, id) {
      for (let index = 0; index < dataSource.length; index++) {
        const element = dataSource[index]
        if (parseInt(element.id) === parseInt(id)) {
          return element
        } else {
          if (element.children === null || element.children.length === 0) {
            continue
          }

          const temp = this.findMenuNode(element.children, id)
          if (temp !== null) {
            return temp
          }
        }
      }

      return null
    },
    /** 删除 */
    handleDelete(id, index) {
      this.$confirm('此操作将永久删除该记录, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        RemoveMenu(id).then(res => {
          if (res.code === 200) {
            this.$notify({
              title: '操作通知',
              message: '成功',
              type: 'success',
              duration: 2000
            })
            this.getList()
          } else {
            this.$notify.error({
              title: '操作通知',
              message: res.msg,
              duration: 2000
            })
          }
        })
      })
    },
    /** 表单操作中，取消按钮的事件处理逻辑 */
    closeFormDialog() {
      this.dialogVisible = false
    },
    /** 提交 新增/修改表单 */
    submit(formname) {
      this.$refs[formname].validate((valid) => {
        if (valid) {
          this.formLoading = true
          this.formData.name = this.trim(this.formData.name)
          this.formData.routerPath = this.trim(this.formData.routerPath)
          this.formData.routerName = this.trim(this.formData.routerName)
          this.formData.icon = this.trim(this.formData.icon)
          this.formData.desc = this.trim(this.formData.desc)
          console.log('父节点选择里面，选择的值为：')
          console.log(this.parentIdArray)
          if (this.parentIdArray.length) {
            this.formData.parentId = parseInt(this.parentIdArray[this.parentIdArray.length - 1])
            this.formData.parentIdStr = this.parentIdArray.join('/')
          } else {
            this.formData.parentId = 0
            this.formData.parentIdStr = ''
          }

          console.log(this.formData)

          if (this.isAddForm) {
            AddMenu(this.formData).then(res => {
              if (res.code === 200) {
                this.$notify({
                  title: '操作通知',
                  message: '成功',
                  type: 'success',
                  duration: 2000
                })
                this.getList()
                this.dialogVisible = false
              } else {
                this.$notify.error({
                  title: '操作通知',
                  message: res.msg,
                  duration: 2000
                })
              }
              this.formLoading = false
            }).catch(res => {
              this.formLoading = false
            })
          } else {
            EditMenu(this.formData).then(res => {
              if (res.code === 200) {
                this.$notify({
                  title: '操作通知',
                  message: '成功',
                  type: 'success',
                  duration: 2000
                })
                this.getList()
                this.dialogVisible = false
              } else {
                this.$notify.error({
                  title: '操作通知',
                  message: res.msg,
                  duration: 2000
                })
              }
              this.formLoading = false
            }).catch(res => {
              this.formLoading = false
            })
          }
        }
      })
    }
  }
}
</script>

<style scoped>
.link-type {
  color: rgb(44, 112, 190);
  cursor: pointer;
}

.el-input::-webkit-scrollbar {
  z-index: 3000;
}
</style>
