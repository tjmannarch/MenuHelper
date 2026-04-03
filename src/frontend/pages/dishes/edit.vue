<template>
	<view class="page">
		<wd-toast />

		<!-- 菜品名称 -->
		<wd-cell-group custom-class="form-group">
			<wd-input label="菜品名称" v-model="form.name" placeholder="请输入菜品名称" required clearable />
		</wd-cell-group>

		<!-- 原材料配置（仅编辑模式） -->
		<template v-if="isEdit">
			<view class="section-header">
				<text class="section-title">原材料配置</text>
				<view class="section-action" @click="openAddIngredient">
					<wd-icon name="add-circle" size="18px" color="#4a90e2" />
					<text class="section-action-text">添加原材料</text>
				</view>
			</view>

			<wd-cell-group v-if="dishIngredients.length > 0" custom-class="form-group">
				<wd-cell v-for="di in dishIngredients" :key="di.id" :title="getIngredientName(di.ingredientId)">
					<template #label>
						<view class="di-label">
							<wd-tag size="small" :type="di.quantityType === 1 ? 'primary' : 'warning'">
								{{ di.quantityType === 1 ? `固定 ${di.fixedQuantity ?? '-'}` : '非固定' }}
							</wd-tag>
						</view>
					</template>
					<template #right-icon>
						<wd-icon name="delete1" size="18px" color="#ee0a24" @click="removeDishIngredient(di.id)" />
					</template>
				</wd-cell>
			</wd-cell-group>
			<view v-else class="empty-ingredients">
				<text>暂未配置原材料</text>
			</view>
		</template>

		<!-- 操作按钮 -->
		<view class="btn-area">
			<wd-button block type="primary" :loading="saving" @click="save">
				{{ isEdit ? '保存名称' : '创建菜品' }}
			</wd-button>
			<wd-button v-if="isEdit" block type="error" plain custom-class="delete-btn"
				@click="confirmDelete">删除菜品</wd-button>
		</view>

		<!-- 添加原材料弹窗 -->
		<wd-popup v-model="showAddSheet" position="bottom"
			custom-style="border-radius: 24rpx 24rpx 0 0; padding-bottom: env(safe-area-inset-bottom);">
			<view class="sheet-header">
				<text class="sheet-title">选择原材料</text>
				<wd-icon name="close" size="20px" color="#999" @click="showAddSheet = false" />
			</view>
			<view class="sheet-content">
				<wd-search v-model="ingredientKeyword" placeholder="搜索原材料" @search="searchIngredients"
					@clear="searchIngredients" />
				<scroll-view scroll-y style="max-height: 600rpx;">
					<wd-cell-group>
						<wd-cell v-for="ing in ingredientOptions" :key="ing.id" :title="ing.name"
							:label="categoryLabel(ing.category)" clickable @click="selectIngredient(ing)" />
					</wd-cell-group>
				</scroll-view>
			</view>
		</wd-popup>

		<!-- 配置用量弹窗 -->
	</view>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()

const id = ref('')
const isEdit = computed(() => !!id.value)

const form = ref({ name: '' })
const saving = ref(false)
const dishIngredients = ref([])
const allIngredients = ref([])

// 添加原材料弹窗
const showAddSheet = ref(false)
const ingredientKeyword = ref('')
const ingredientOptions = ref([])
const pendingIngredient = ref(null)

const CATEGORY_MAP = { 1: '凉皮类', 2: '肉夹馍类', 3: '石锅饭类', 4: '通用食材' }
const categoryLabel = (v) => CATEGORY_MAP[v] || ''

function getIngredientName(ingredientId) {
	return allIngredients.value.find(i => i.id === ingredientId)?.name || ingredientId
}

async function loadDetail() {
	try {
		const data = await api.get(`/api/dishes/${id.value}`)
		form.value.name = data.name
		dishIngredients.value = data.dishIngredients ?? []
	} catch (e) {
		toast.error(e.message)
	}
}

async function loadAllIngredients() {
	try {
		const res = await api.get('/api/ingredients', { pageSize: 100 })
		allIngredients.value = res.items ?? []
		ingredientOptions.value = allIngredients.value
	} catch (e) { /* ignore */ }
}

function searchIngredients() {
	const kw = ingredientKeyword.value.toLowerCase()
	ingredientOptions.value = kw
		? allIngredients.value.filter(i => i.name.includes(kw))
		: allIngredients.value
}

function openAddIngredient() {
	ingredientKeyword.value = ''
	ingredientOptions.value = allIngredients.value
	showAddSheet.value = true
}

async function selectIngredient(ing) {
	showAddSheet.value = false
	// 等弹窗关闭动画完成
	await new Promise(r => setTimeout(r, 300))

	uni.showModal({
		title: `配置 "${ing.name}" 用量`,
		editable: true,
		placeholderText: '请输入固定用量（留空表示非固定用量）',
		success: async ({ confirm, content: inputVal }) => {
			if (!confirm) return
			const fixedQty = (inputVal && inputVal.trim() !== '') ? parseFloat(inputVal) : null
			console.log(fixedQty)
			const quantityType = fixedQty != null ? 1 : 2
			try {
				await api.post(`/api/dishes/${id.value}/ingredients`, {
					ingredientId: ing.id,
					quantityType,
					fixedQuantity: fixedQty
				})
				toast.success('已添加')
				await loadDetail()
			} catch (e) {
				toast.error(e?.message || '添加失败')
			}
		}
	})
}

async function removeDishIngredient(diId) {
	uni.showModal({
		title: '确认移除',
		content: '确认移除此原材料？',
		success: async ({ confirm }) => {
			if (!confirm) return
			try {
				await api.delete(`/api/dishes/${id.value}/ingredients/${diId}`)
				toast.success('已移除')
				await loadDetail()
			} catch (e) {
				toast.error(e.message)
			}
		}
	})
}

async function save() {
	if (!form.value.name.trim()) return toast.warning('请输入菜品名称')
	saving.value = true
	try {
		if (isEdit.value) {
			await api.put(`/api/dishes/${id.value}`, { name: form.value.name.trim() })
			toast.success('更新成功')
		} else {
			await api.post('/api/dishes', { name: form.value.name.trim() })
			toast.success('创建成功，可继续配置原材料')
			setTimeout(() => uni.navigateBack(), 1000)
		}
	} catch (e) {
		toast.error(e.message)
	} finally {
		saving.value = false
	}
}

function confirmDelete() {
	uni.showModal({
		title: '确认删除',
		content: `确认删除"${form.value.name}"吗？`,
		success: async ({ confirm }) => {
			if (!confirm) return
			try {
				await api.delete(`/api/dishes/${id.value}`)
				toast.success('已删除')
				setTimeout(() => uni.navigateBack(), 800)
			} catch (e) {
				toast.error(e.message)
			}
		}
	})
}

onMounted(() => {
	const pages = getCurrentPages()
	const current = pages[pages.length - 1]
	id.value = current.$page?.options?.id || ''
	uni.setNavigationBarTitle({ title: isEdit.value ? '编辑菜品' : '新增菜品' })
	if (isEdit.value) {
		loadDetail()
		loadAllIngredients()
	}
})
</script>

<style lang="scss">
.page {
	background: #f5f5f5;
	min-height: 100vh;
	padding-bottom: 60rpx;
}

:deep(.form-group) {
	margin-bottom: 16rpx;
}

.section-header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	padding: 24rpx 32rpx 12rpx;
}

.section-title {
	font-size: 26rpx;
	color: #999;
}

.section-action {
	display: flex;
	align-items: center;
	gap: 6rpx;

	.section-action-text {
		font-size: 26rpx;
		color: #4a90e2;
	}
}

.di-label {
	margin-top: 6rpx;
}

.empty-ingredients {
	text-align: center;
	padding: 40rpx;
	font-size: 26rpx;
	color: #ccc;
}

.btn-area {
	padding: 24rpx 32rpx;

	:deep(.delete-btn) {
		margin-top: 16rpx;
	}
}

.sheet-header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	padding: 24rpx 32rpx 8rpx;

	.sheet-title {
		font-size: 30rpx;
		font-weight: bold;
		color: #333;
	}
}

.sheet-content {
	padding: 0 16rpx 16rpx;
}
</style>
