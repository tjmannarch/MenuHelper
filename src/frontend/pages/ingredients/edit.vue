<template>
	<view class="page">
		<wd-toast />

		<wd-cell-group custom-class="form-group">
			<!-- 名称 -->
			<wd-input label="名称" v-model="form.name" placeholder="请输入原材料名称" required clearable />

			<!-- 计量单位 -->
			<wd-input label="单位" v-model="form.unit" placeholder="如：斤、个、包" required clearable />

			<!-- 菜品分类 -->
			<wd-picker v-model="form.category" :columns="categoryColumns" label="菜品分类" placeholder="请选择" required />

			<!-- 消耗方式 -->
			<wd-picker v-model="form.consumptionType" :columns="consumptionColumns" label="消耗方式" placeholder="请选择"
				required />

			<!-- 默认单价 -->
			<wd-input label="默认单价" v-model="form.defaultUnitPrice" type="digit" placeholder="暂不知道可留空" />
		</wd-cell-group>

		<view class="section-title">补货设置（选填）</view>

		<wd-cell-group custom-class="form-group">
			<!-- 安全库存线 -->
			<wd-input label="安全库存线" v-model="form.safetyStockLevel" type="digit" placeholder="低于此值提醒补货" />

			<!-- 备货周期 -->
			<wd-input label="备货周期(天)" v-model="form.restockCycleDays" type="number" placeholder="默认进货间隔天数" />

			<!-- 最长存放天数 -->
			<wd-input label="最长存放(天)" v-model="form.maxShelfDays" type="number" placeholder="超期触发新鲜度预警" />
		</wd-cell-group>

		<!-- 操作按钮 -->
		<view class="btn-area">
			<wd-button block type="primary" :loading="saving" @click="save">保存</wd-button>
			<wd-button v-if="isEdit" block type="error" plain custom-class="delete-btn"
				@click="confirmDelete">删除</wd-button>
		</view>
	</view>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()

// 路由参数
const id = ref('')
const isEdit = computed(() => !!id.value)

const form = ref({
	name: '',
	unit: '',
	category: '',
	consumptionType: '',
	defaultUnitPrice: '',
	safetyStockLevel: '',
	restockCycleDays: '',
	maxShelfDays: ''
})

const saving = ref(false)

// 从接口动态加载的枚举数据
const categoryColumns = ref([])
const consumptionColumns = ref([])

async function loadMeta() {
	try {
		const res = await api.get('/api/meta/ingredients')
		categoryColumns.value = (res.categories ?? []).map(c => ({ label: c.label, value: c.value }))
		consumptionColumns.value = (res.consumptionTypes ?? []).map(c => ({ label: c.label, value: c.value }))
	} catch (e) { /* 降级为空 */ }
}

async function loadDetail() {
	try {
		const data = await api.get(`/api/ingredients/${id.value}`)
		form.value = {
			name: data.name,
			unit: data.unit,
			category: data.category,
			consumptionType: data.consumptionType,
			defaultUnitPrice: String(data.defaultUnitPrice),
			safetyStockLevel: data.safetyStockLevel != null ? String(data.safetyStockLevel) : '',
			restockCycleDays: data.restockCycleDays != null ? String(data.restockCycleDays) : '',
			maxShelfDays: data.maxShelfDays != null ? String(data.maxShelfDays) : ''
		}
	} catch (e) {
		toast.error(e.message)
	}
}

async function save() {
	if (!form.value.name.trim()) return toast.warning('请输入名称')
	if (!form.value.unit.trim()) return toast.warning('请输入单位')
	if (!form.value.category) return toast.warning('请选择菜品分类')
	if (!form.value.consumptionType) return toast.warning('请选择消耗方式')

	saving.value = true
	const payload = {
		name: form.value.name.trim(),
		unit: form.value.unit.trim(),
		category: Number(form.value.category),
		consumptionType: Number(form.value.consumptionType),
		defaultUnitPrice: parseFloat(form.value.defaultUnitPrice),
		safetyStockLevel: form.value.safetyStockLevel ? parseFloat(form.value.safetyStockLevel) : null,
		restockCycleDays: form.value.restockCycleDays ? parseInt(form.value.restockCycleDays) : null,
		maxShelfDays: form.value.maxShelfDays ? parseInt(form.value.maxShelfDays) : null
	}
	try {
		if (isEdit.value) {
			await api.put(`/api/ingredients/${id.value}`, payload)
			toast.success('更新成功')
		} else {
			await api.post('/api/ingredients', payload)
			toast.success('创建成功')
		}
		setTimeout(() => uni.navigateBack(), 1000)
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
				await api.delete(`/api/ingredients/${id.value}`)
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
	uni.setNavigationBarTitle({ title: isEdit.value ? '编辑原材料' : '新增原材料' })
	loadMeta()
	if (isEdit.value) loadDetail()
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

.section-title {
	font-size: 26rpx;
	color: #999;
	padding: 24rpx 32rpx 12rpx;
}

.btn-area {
	padding: 24rpx 32rpx;

	:deep(.delete-btn) {
		margin-top: 16rpx;
	}
}
</style>
